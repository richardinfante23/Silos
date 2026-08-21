-- Configuración Tasa diaria ($/ton y dólar en Bs)
-- Relación con recepción/despacho: por fecha (JOIN ON fecha = tasa_diaria.fecha)

CREATE TABLE IF NOT EXISTS public.tasa_diaria (
    id SERIAL PRIMARY KEY,
    fecha date NOT NULL,
    precio_tonelada_usd numeric(18,4) NOT NULL DEFAULT 0,
    tasa_dolar_bs numeric(18,4) NOT NULL DEFAULT 0,
    creado_en timestamp without time zone NOT NULL DEFAULT now(),
    actualizado_en timestamp without time zone NOT NULL DEFAULT now(),
    CONSTRAINT uq_tasa_diaria_fecha UNIQUE (fecha)
);

COMMENT ON TABLE public.tasa_diaria IS 'Tasa diaria: precio USD por tonelada y valor del dólar en Bs';
COMMENT ON COLUMN public.tasa_diaria.precio_tonelada_usd IS 'Precio diario de la tonelada en USD';
COMMENT ON COLUMN public.tasa_diaria.tasa_dolar_bs IS 'Precio diario del dólar en bolívares';

CREATE OR REPLACE FUNCTION public.buscartasadiaria(ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT id, fecha, precio_tonelada_usd, tasa_dolar_bs, creado_en, actualizado_en
        FROM tasa_diaria
        ORDER BY fecha DESC, id DESC;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.buscartasadiariaxid(prmid integer, ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT id, fecha, precio_tonelada_usd, tasa_dolar_bs, creado_en, actualizado_en
        FROM tasa_diaria
        WHERE id = prmid;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.buscartasadiariaxfecha(prmfecha date, ref refcursor)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    OPEN ref FOR
        SELECT id, fecha, precio_tonelada_usd, tasa_dolar_bs, creado_en, actualizado_en
        FROM tasa_diaria
        WHERE fecha = prmfecha;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.spinsertartasadiaria(
    prmfecha date,
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
    IF EXISTS (SELECT 1 FROM tasa_diaria WHERE fecha = prmfecha) THEN
        RAISE EXCEPTION 'Ya existe una tasa registrada para la fecha %', prmfecha
            USING ERRCODE = '23505';
    END IF;

    INSERT INTO tasa_diaria (fecha, precio_tonelada_usd, tasa_dolar_bs)
    VALUES (prmfecha, prmprecio_tonelada_usd, prmtasa_dolar_bs)
    RETURNING id INTO varid;

    OPEN ref FOR
        SELECT id, fecha, precio_tonelada_usd, tasa_dolar_bs, creado_en, actualizado_en
        FROM tasa_diaria
        WHERE id = varid;
    RETURN ref;
END;
$$;

CREATE OR REPLACE FUNCTION public.spactualizartasadiaria(
    prmid integer,
    prmprecio_tonelada_usd numeric,
    prmtasa_dolar_bs numeric,
    ref refcursor
)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE tasa_diaria
    SET precio_tonelada_usd = prmprecio_tonelada_usd,
        tasa_dolar_bs = prmtasa_dolar_bs,
        actualizado_en = now()
    WHERE id = prmid;

    OPEN ref FOR
        SELECT id, fecha, precio_tonelada_usd, tasa_dolar_bs, creado_en, actualizado_en
        FROM tasa_diaria
        WHERE id = prmid;
    RETURN ref;
END;
$$;
