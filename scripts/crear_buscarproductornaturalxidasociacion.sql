-- Función faltante en el backup: productores/destinos por asociación
-- Usada al eliminar asociaciones (asociacionForm / ProductorData / destinoData)

CREATE OR REPLACE FUNCTION public.buscarproductornaturalxidasociacion(
    prmid_asociacion integer,
    ref refcursor
) RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
    SELECT id, nombre, id_asociacion, eliminado
    FROM (
        SELECT id, nombre, id_asociacion, eliminado
        FROM productor_natural
        WHERE id_asociacion = prmid_asociacion
          AND COALESCE(eliminado, false) = false
        UNION ALL
        SELECT id, nombre, id_asociacion, eliminado
        FROM productor_juridico
        WHERE id_asociacion = prmid_asociacion
          AND COALESCE(eliminado, false) = false
        UNION ALL
        SELECT id, nombre, id_asociacion, eliminado
        FROM destino_natural
        WHERE id_asociacion = prmid_asociacion
          AND COALESCE(eliminado, false) = false
    ) t;
    RETURN ref;
END;
$$;
