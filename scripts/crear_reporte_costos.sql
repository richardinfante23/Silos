-- Reporte de costos/recepciones con tasa del día por producto
CREATE OR REPLACE FUNCTION public.buscarreportecostos(
    prmfecha_desde date,
    prmfecha_hasta date,
    prmid_producto integer,
    prmid_productor integer,
    prmid_productor_juridico integer,
    ref refcursor
)
RETURNS refcursor
LANGUAGE plpgsql
AS $$
BEGIN
    IF prmid_producto = 0 THEN
        prmid_producto := NULL;
    END IF;
    IF prmid_productor = 0 THEN
        prmid_productor := NULL;
    END IF;
    IF prmid_productor_juridico = 0 THEN
        prmid_productor_juridico := NULL;
    END IF;

    OPEN ref FOR
        SELECT
            r.id AS ticket,
            r.fecha,
            COALESCE(pr.nombre, '') AS producto,
            CASE
                WHEN r.id_productor IS NOT NULL AND r.id_productor > 0 THEN
                    COALESCE(pn.letra::text, '') || '-' || COALESCE(pn.cedula::text, '') || ' ' || COALESCE(pn.nombre, '')
                ELSE
                    COALESCE(pj.letra::text, '') || '-' || COALESCE(pj.rif::text, '') || ' ' || COALESCE(pj.nombre, '')
            END AS productor,
            COALESCE(NULLIF(COALESCE(r.peso_bruto, 0) - COALESCE(r.peso_tara, 0), 0), 0) AS peso_neto,
            COALESCE(t.precio_tonelada_usd, 0) AS precio_tonelada_usd,
            COALESCE(t.tasa_dolar_bs, 0) AS precio_bs_usd,
            ROUND(
                ((COALESCE(r.peso_bruto, 0) - COALESCE(r.peso_tara, 0)) / 1000.0)
                * COALESCE(t.precio_tonelada_usd, 0)
            , 2) AS total_usd,
            ROUND(
                ((COALESCE(r.peso_bruto, 0) - COALESCE(r.peso_tara, 0)) / 1000.0)
                * COALESCE(t.precio_tonelada_usd, 0)
                * COALESCE(t.tasa_dolar_bs, 0)
            , 2) AS total_bs
        FROM recepciones r
        LEFT JOIN productos pr ON pr.id = r.id_producto
        LEFT JOIN productor_natural pn ON pn.id = r.id_productor
        LEFT JOIN productor_juridico pj ON pj.id = r.id_productor_juridico
        LEFT JOIN tasa_diaria t
            ON t.fecha = r.fecha
           AND t.id_producto = r.id_producto
        WHERE r.id_estatus = 5
          AND (prmfecha_desde IS NULL OR r.fecha >= prmfecha_desde)
          AND (prmfecha_hasta IS NULL OR r.fecha <= prmfecha_hasta)
          AND (prmid_producto IS NULL OR r.id_producto = prmid_producto)
          AND (prmid_productor IS NULL OR r.id_productor = prmid_productor)
          AND (prmid_productor_juridico IS NULL OR r.id_productor_juridico = prmid_productor_juridico)
        ORDER BY r.fecha DESC, r.id DESC;

    RETURN ref;
END;
$$;
