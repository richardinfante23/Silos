-- =============================================================================
-- Analisis / refuerzo: stores con parametro prmeliminar
-- Base: produccion_cliente_
-- Fecha: 2026-08-14
-- =============================================================================
-- Hallazgo:
--   Los 5 SP con "prmeliminar" escriben la columna "eliminado" (no "eliminar").
--   En esta BD las 5 tablas YA tienen "eliminado".
--   El error 42883 suele ser por nombre de parametro en el cliente
--   (C# envia :prmeliminado y el SP espera prmeliminar), NO por falta de columna.
-- =============================================================================

BEGIN;

-- 1) Asegurar columna eliminado (idempotente; no rompe si ya existe)
ALTER TABLE public.silos
    ADD COLUMN IF NOT EXISTS eliminado boolean DEFAULT false;

ALTER TABLE public.transportes
    ADD COLUMN IF NOT EXISTS eliminado boolean DEFAULT false;

ALTER TABLE public.asociaciones
    ADD COLUMN IF NOT EXISTS eliminado boolean DEFAULT false;

ALTER TABLE public.productos
    ADD COLUMN IF NOT EXISTS eliminado boolean DEFAULT false;

ALTER TABLE public.unidad
    ADD COLUMN IF NOT EXISTS eliminado boolean DEFAULT false;

-- Normalizar NULLs y default en unidad (historicamente sin default)
UPDATE public.unidad SET eliminado = false WHERE eliminado IS NULL;
ALTER TABLE public.unidad ALTER COLUMN eliminado SET DEFAULT false;

UPDATE public.silos SET eliminado = false WHERE eliminado IS NULL;
UPDATE public.transportes SET eliminado = false WHERE eliminado IS NULL;
UPDATE public.asociaciones SET eliminado = false WHERE eliminado IS NULL;
UPDATE public.productos SET eliminado = false WHERE eliminado IS NULL;

COMMIT;

-- =============================================================================
-- 2) Verificacion: stores prmeliminar vs columna eliminado
-- =============================================================================
SELECT
    p.proname AS store,
    pg_get_function_identity_arguments(p.oid) AS parametros,
    CASE p.proname
        WHEN 'spactualizarasilos' THEN 'silos'
        WHEN 'spactualizarasociaciones' THEN 'asociaciones'
        WHEN 'spactualizarproductos' THEN 'productos'
        WHEN 'spactualizartransportes' THEN 'transportes'
        WHEN 'spactualizarunidades' THEN 'unidad'
    END AS tabla,
    CASE
        WHEN EXISTS (
            SELECT 1
            FROM information_schema.columns c
            WHERE c.table_schema = 'public'
              AND c.column_name = 'eliminado'
              AND c.table_name = CASE p.proname
                    WHEN 'spactualizarasilos' THEN 'silos'
                    WHEN 'spactualizarasociaciones' THEN 'asociaciones'
                    WHEN 'spactualizarproductos' THEN 'productos'
                    WHEN 'spactualizartransportes' THEN 'transportes'
                    WHEN 'spactualizarunidades' THEN 'unidad'
                 END
        ) THEN 'OK - tiene eliminado'
        ELSE 'FALTA columna eliminado'
    END AS estado_columna
FROM pg_proc p
JOIN pg_namespace n ON n.oid = p.pronamespace
WHERE n.nspname = 'public'
  AND pg_get_function_identity_arguments(p.oid) ILIKE '%prmeliminar%'
ORDER BY 1;

-- =============================================================================
-- Mapeo resumido (referencia)
-- =============================================================================
-- Store                        | Param SP      | Columna UPDATE | Tabla
-- -----------------------------+---------------+----------------+--------------
-- spactualizarasilos           | prmeliminar   | eliminado      | silos
-- spactualizarasociaciones     | prmeliminar   | eliminado      | asociaciones
-- spactualizarproductos        | prmeliminar   | eliminado      | productos
-- spactualizartransportes      | prmeliminar   | eliminado      | transportes
-- spactualizarunidades         | prmeliminar   | eliminado      | unidad
-- =============================================================================
