CREATE OR REPLACE FUNCTION public.buscarperfil(ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT * FROM perfil
        WHERE id IN (1, 2)
        ORDER BY id;
    RETURN ref;
END;
$$;

UPDATE perfil
SET nombre = 'Administrador'
WHERE id = 1 AND nombre ILIKE 'Aministrador';
