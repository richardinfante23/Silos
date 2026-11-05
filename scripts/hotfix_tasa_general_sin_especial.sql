-- =============================================================================
-- HOTFIX PRODUCCIÓN (mínimo invasivo)
-- Problema: existen tasas del día pero con es_especial = true,
--           y la recepción solo acepta tasa GENERAL (es_especial = false).
-- Efecto:   "No hay tasa configurada para el producto...".
-- Solución: si para una fecha+producto NO hay general, promover la de menor id.
-- =============================================================================

BEGIN;

-- 1) Asegurar columna (por si la BD aún no la tiene)
ALTER TABLE public.tasa_diaria
    ADD COLUMN IF NOT EXISTS es_especial boolean NOT NULL DEFAULT false;

-- 2) Promover a GENERAL cuando no exista ninguna general esa fecha/producto
UPDATE public.tasa_diaria t
SET es_especial = false
WHERE t.es_especial = true
  AND NOT EXISTS (
      SELECT 1
      FROM public.tasa_diaria g
      WHERE g.fecha = t.fecha
        AND g.id_producto = t.id_producto
        AND g.es_especial = false
  )
  AND t.id = (
      SELECT MIN(t2.id)
      FROM public.tasa_diaria t2
      WHERE t2.fecha = t.fecha
        AND t2.id_producto = t.id_producto
  );

COMMIT;

-- Verificación (debe mostrar generales de hoy):
-- SELECT id, fecha, id_producto, es_especial
-- FROM tasa_diaria
-- WHERE fecha = CURRENT_DATE
-- ORDER BY id_producto, id;
