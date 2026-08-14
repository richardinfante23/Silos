-- Opcional: mejora nombres de transporte poco claros y el listado de conductores.
-- Ejecutar en produccion_cliente_ si se desea corregir también a nivel BD.

UPDATE public.transportes
SET nombre = 'NO APLICA'
WHERE id = 8 AND nombre = '0';

CREATE OR REPLACE FUNCTION public.buscarconductoresgeneral(ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
  OPEN ref FOR
  SELECT c.*,
    CASE
      WHEN COALESCE(NULLIF(btrim(t.nombre), ''), '0') IN ('', '0') THEN
        COALESCE(NULLIF(btrim(t.rif), ''), 'Transporte #' || t.id::text)
      WHEN upper(COALESCE(btrim(t.rif), '')) IN ('', 'NO APLICA') THEN
        btrim(t.nombre)
      ELSE
        btrim(t.nombre) || ' — ' || btrim(t.rif)
    END AS "Transporte"
  FROM conductores c
  INNER JOIN transportes t ON t.id = c.id_transporte
  WHERE c.eliminado = false;
  RETURN ref;
END;
$$;
