-- Precio tonelada específico para modalidad 50/50
-- Ejecutar una vez en la BD del cliente (PostgreSQL).

ALTER TABLE public.tasa_diaria
    ADD COLUMN IF NOT EXISTS precio_50_50_usd numeric(18,4) NOT NULL DEFAULT 0;

COMMENT ON COLUMN public.tasa_diaria.precio_50_50_usd IS 'Precio tonelada $ para modalidad 50/50 (mitad en efectivo)';

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
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
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
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
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
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
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
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
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

DROP FUNCTION IF EXISTS public.spinsertartasadiaria(date, integer, numeric, numeric, numeric, refcursor);
CREATE OR REPLACE FUNCTION public.spinsertartasadiaria(
    prmfecha date,
    prmid_producto integer,
    prmprecio_tonelada_usd numeric,
    prmtasa_dolar_bs numeric,
    prmprecio_efectivo_usd numeric,
    prmprecio_50_50_usd numeric,
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

    INSERT INTO tasa_diaria (
        fecha, id_producto, precio_tonelada_usd, tasa_dolar_bs,
        precio_efectivo_usd, precio_50_50_usd
    )
    VALUES (
        prmfecha, prmid_producto, prmprecio_tonelada_usd, prmtasa_dolar_bs,
        prmprecio_efectivo_usd, prmprecio_50_50_usd
    )
    RETURNING id INTO varid;

    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.id = varid;
    RETURN ref;
END;
$$;

DROP FUNCTION IF EXISTS public.spactualizartasadiaria(integer, integer, numeric, numeric, numeric, refcursor);
CREATE OR REPLACE FUNCTION public.spactualizartasadiaria(
    prmid integer,
    prmid_producto integer,
    prmprecio_tonelada_usd numeric,
    prmtasa_dolar_bs numeric,
    prmprecio_efectivo_usd numeric,
    prmprecio_50_50_usd numeric,
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
        precio_efectivo_usd = prmprecio_efectivo_usd,
        precio_50_50_usd = prmprecio_50_50_usd,
        actualizado_en = now()
    WHERE id = prmid;

    OPEN ref FOR
        SELECT
            t.id,
            t.fecha,
            t.id_producto,
            COALESCE(p.nombre, '') AS producto,
            t.precio_tonelada_usd,
            t.precio_efectivo_usd,
            t.precio_50_50_usd,
            t.tasa_dolar_bs,
            t.creado_en,
            t.actualizado_en
        FROM tasa_diaria t
        LEFT JOIN productos p ON p.id = t.id_producto
        WHERE t.id = prmid;
    RETURN ref;
END;
$$;
