-- =============================================================================
-- PRODUCCIÓN: Tasa especial / tasa general (arreglo final)
-- Ejecutar una vez en PostgreSQL de producción.
-- =============================================================================
--
-- Qué hace:
--   1) Agrega recepciones.id_tasa (asociación recepción → tasa)
--   2) Agrega tasa_diaria.es_especial (false=general, true=especial)
--   3) Quita id_recepcion si existía (ya no se usa)
--   4) Marca como especiales las tasas ya ligadas a recepción
--   5) Si hay varias "generales" misma fecha+producto, deja 1 (menor id)
--      y marca el resto como especial
--   6) Crea el unique SOLO para generales (ya sin duplicados)
--
-- =============================================================================

BEGIN;

-- 1) Asociación en recepción
ALTER TABLE public.recepciones
    ADD COLUMN IF NOT EXISTS id_tasa integer NULL;

COMMENT ON COLUMN public.recepciones.id_tasa IS
    'Id de tasa_diaria. NULL = aún no asociada; la app usa general o especial según corresponda.';

-- 2) Marca general / especial
ALTER TABLE public.tasa_diaria
    ADD COLUMN IF NOT EXISTS es_especial boolean NOT NULL DEFAULT false;

COMMENT ON COLUMN public.tasa_diaria.es_especial IS
    'false = tasa general del día; true = tasa especial.';

-- 3) Limpiar intento anterior (id_recepcion)
ALTER TABLE public.tasa_diaria DROP CONSTRAINT IF EXISTS fk_tasa_diaria_recepcion;
DROP INDEX IF EXISTS ix_tasa_diaria_id_recepcion;
ALTER TABLE public.tasa_diaria DROP COLUMN IF EXISTS id_recepcion;

-- 4) Si la tasa ya está en recepciones.id_tasa → es especial
UPDATE public.tasa_diaria t
SET es_especial = true
WHERE EXISTS (
    SELECT 1
    FROM public.recepciones r
    WHERE r.id_tasa = t.id
);

-- 5) Duplicados de generales (misma fecha + producto):
--    deja la de menor id como general; el resto → especial
UPDATE public.tasa_diaria t
SET es_especial = true
FROM (
    SELECT fecha, id_producto, MIN(id) AS id_general
    FROM public.tasa_diaria
    WHERE es_especial = false
    GROUP BY fecha, id_producto
    HAVING COUNT(*) > 1
) d
WHERE t.fecha = d.fecha
  AND t.id_producto = d.id_producto
  AND t.es_especial = false
  AND t.id <> d.id_general;

-- 6) Unique solo para generales
ALTER TABLE public.tasa_diaria DROP CONSTRAINT IF EXISTS uq_tasa_diaria_fecha_producto;
DROP INDEX IF EXISTS uq_tasa_diaria_fecha_producto;
DROP INDEX IF EXISTS ux_tasa_diaria_fecha_producto_general;

CREATE UNIQUE INDEX ux_tasa_diaria_fecha_producto_general
    ON public.tasa_diaria (fecha, id_producto)
    WHERE NOT es_especial;

-- 7) FK recepción → tasa (opcional; si falla por datos, comentar este bloque)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'fk_recepciones_tasa'
    ) THEN
        ALTER TABLE public.recepciones
            ADD CONSTRAINT fk_recepciones_tasa
            FOREIGN KEY (id_tasa) REFERENCES public.tasa_diaria(id);
    END IF;
END $$;

COMMIT;

-- Verificación rápida:
-- SELECT id, fecha, id_producto, es_especial FROM tasa_diaria ORDER BY fecha DESC, id;
-- SELECT fecha, id_producto, COUNT(*) 
-- FROM tasa_diaria WHERE es_especial = false
-- GROUP BY fecha, id_producto HAVING COUNT(*) > 1;
-- (la segunda consulta debe devolver 0 filas)
