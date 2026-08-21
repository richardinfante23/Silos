-- Tasa diaria por producto
ALTER TABLE public.tasa_diaria
    ADD COLUMN IF NOT EXISTS id_producto integer;

UPDATE public.tasa_diaria t
SET id_producto = (SELECT MIN(id) FROM productos WHERE COALESCE(eliminado, false) = false)
WHERE t.id_producto IS NULL;

ALTER TABLE public.tasa_diaria
    ALTER COLUMN id_producto SET NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'fk_tasa_diaria_producto'
    ) THEN
        ALTER TABLE public.tasa_diaria
            ADD CONSTRAINT fk_tasa_diaria_producto
            FOREIGN KEY (id_producto) REFERENCES public.productos(id);
    END IF;
END $$;

ALTER TABLE public.tasa_diaria DROP CONSTRAINT IF EXISTS uq_tasa_diaria_fecha;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_tasa_diaria_fecha_producto'
    ) THEN
        ALTER TABLE public.tasa_diaria
            ADD CONSTRAINT uq_tasa_diaria_fecha_producto UNIQUE (fecha, id_producto);
    END IF;
END $$;

CREATE OR REPLACE FUNCTION public.buscartasadiaria(ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        ORDER BY t.fecha DESC, p.nombre ASC, t.id DESC;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.buscartasadiariaxid(prmid integer, ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.id = prmid;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.buscartasadiariaxfecha(prmfecha date, ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.fecha = prmfecha
        ORDER BY p.nombre ASC;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.buscartasadiariaxfechaproducto(
    prmfecha date,
    prmid_producto integer,
    ref refcursor
)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.fecha = prmfecha
          AND t.id_producto = prmid_producto;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.spinsertartasadiaria(
    prmfecha date,
    prmid_producto integer,
    prmprecio_tonelada_usd numeric,
    prmtasa_dolar_bs numeric,
    ref refcursor
)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
DECLARE
    varid integer;
BEGIN
    IF EXISTS (
        SELECT 1 FROM tasa_diaria
        WHERE fecha = prmfecha AND id_producto = prmid_producto
    ) THEN
        RAISE EXCEPTION 'Ya existe una tasa registrada para la fecha % y el producto indicado', prmfecha
            USING ERRCODE = '23505';
    END IF;

    INSERT INTO tasa_diaria (fecha, id_producto, precio_tonelada_usd, tasa_dolar_bs)
    VALUES (prmfecha, prmid_producto, prmprecio_tonelada_usd, prmtasa_dolar_bs)
    RETURNING id INTO varid;

    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.id = varid;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.spactualizartasadiaria(
    prmid integer,
    prmid_producto integer,
    prmprecio_tonelada_usd numeric,
    prmtasa_dolar_bs numeric,
    ref refcursor
)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE tasa_diaria
    SET id_producto = prmid_producto,
        precio_tonelada_usd = prmprecio_tonelada_usd,
        tasa_dolar_bs = prmtasa_dolar_bs,
        actualizado_en = now()
    WHERE id = prmid;

    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.id = prmid;
    RETURN ref;
END;
$$;
