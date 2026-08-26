--
-- PostgreSQL database dump
--

\restrict MnjuDMftlgqP6LiNyXIf1M3Hfum4seXEtKEpnglPO5sfiDjtAXr03QS8uxOhGQ7

-- Dumped from database version 18.6
-- Dumped by pg_dump version 18.6

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: perfil; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.perfil VALUES (2, 'Romana');
INSERT INTO public.perfil VALUES (3, 'Analisis');
INSERT INTO public.perfil VALUES (4, 'Taquilla');
INSERT INTO public.perfil VALUES (5, 'Gerente');
INSERT INTO public.perfil VALUES (6, 'Supervisor');
INSERT INTO public.perfil VALUES (1, 'Administrador');


--
-- Data for Name: unidad; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.unidad VALUES (1, 'CAJAS', false);


--
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.usuarios VALUES (14, 'ADMIN', 'MQAyADMA', 'Richard Developer', 1, false);
INSERT INTO public.usuarios VALUES (2, 'ROMANA', 'MQAyADMA', 'Romana Prueba', 2, false);


--
-- Data for Name: articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: asociaciones; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.asociaciones VALUES (4, 'Unica', '2233', false);


--
-- Data for Name: transportes; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.transportes VALUES (1, 'Unica', '5555', false);


--
-- Data for Name: conductores; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.conductores VALUES (3, 22, 'Roman', 1, true);
INSERT INTO public.conductores VALUES (2, 223366, 'Jose Perez Prueba', 1, false);


--
-- Data for Name: destino_natural; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.destino_natural VALUES (1, 'Cliente Prueba', 123, 'V ', 4, false);


--
-- Data for Name: destinos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.destinos VALUES (1, 'Remanvenca', false, 4, 'J', 23666);
INSERT INTO public.destinos VALUES (2, 'Prueba', false, 4, 'J', 123);


--
-- Data for Name: estatntes_articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.estatntes_articulos VALUES (8, '1', false);


--
-- Data for Name: estatus_despachos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.estatus_despachos VALUES (1, 'Control Previo');
INSERT INTO public.estatus_despachos VALUES (4, 'Pesada Salida');
INSERT INTO public.estatus_despachos VALUES (5, 'Culminado');
INSERT INTO public.estatus_despachos VALUES (6, 'Anulado');
INSERT INTO public.estatus_despachos VALUES (7, 'Piso');
INSERT INTO public.estatus_despachos VALUES (2, 'Pesada  Entrada');
INSERT INTO public.estatus_despachos VALUES (3, 'Análisis Laboratorio');


--
-- Data for Name: estatus_recepcion; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.estatus_recepcion VALUES (1, 'Control Previo');
INSERT INTO public.estatus_recepcion VALUES (2, 'Análisis Laboratorio');
INSERT INTO public.estatus_recepcion VALUES (3, 'Pesada Entrada');
INSERT INTO public.estatus_recepcion VALUES (4, 'Pesada Salida');
INSERT INTO public.estatus_recepcion VALUES (5, 'Culminado');
INSERT INTO public.estatus_recepcion VALUES (6, 'Anulado');
INSERT INTO public.estatus_recepcion VALUES (7, 'Piso');


--
-- Data for Name: item_analisis; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.item_analisis VALUES (1, 'HUMEDAD');
INSERT INTO public.item_analisis VALUES (2, 'IMPUREZAS');
INSERT INTO public.item_analisis VALUES (3, 'GRANOS CON GERMEN DAÑADO');
INSERT INTO public.item_analisis VALUES (4, 'GRANOS DAÑADOS POR CALOR');
INSERT INTO public.item_analisis VALUES (5, 'GRANOS INFECTADOS (INSECTOS)');
INSERT INTO public.item_analisis VALUES (6, 'GRANOS INFECTADOS (MICROORG)');
INSERT INTO public.item_analisis VALUES (7, 'GRANOS DAÑADOS TOTALES (G.D.T.)');
INSERT INTO public.item_analisis VALUES (8, 'GRANOS PARTIDOS');
INSERT INTO public.item_analisis VALUES (9, 'GRANOS CRISTALIZADOS');
INSERT INTO public.item_analisis VALUES (10, 'MEZCLA DE COLOR');
INSERT INTO public.item_analisis VALUES (11, 'PESO VOLUMETRICO');
INSERT INTO public.item_analisis VALUES (12, 'AFLATOXINAS');
INSERT INTO public.item_analisis VALUES (13, 'NUMERO DE INSECTOS VIVOS');
INSERT INTO public.item_analisis VALUES (14, 'GRANOS QUEMADOS');
INSERT INTO public.item_analisis VALUES (15, 'SENSORIAL');


--
-- Data for Name: productos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.productos VALUES (2, 'Maiz Amarillo', false);
INSERT INTO public.productos VALUES (1, 'Maiz Blanco', false);


--
-- Data for Name: item_producto_analisis; Type: TABLE DATA; Schema: public; Owner: postgres
--



--
-- Data for Name: productor_juridico; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.productor_juridico VALUES (1, 'J', 4, false, 'Rongrat', 123);


--
-- Data for Name: productor_natural; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.productor_natural VALUES (1, 'Richard Prueba', 223366, 'V ', 4, false);
INSERT INTO public.productor_natural VALUES (3, 'Jose Bello Prueba', 22336, 'V ', 4, false);
INSERT INTO public.productor_natural VALUES (2, 'Productor Prueba', 123, 'V ', 4, false);


--
-- Data for Name: seccion_articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.seccion_articulos VALUES (9, 'F', 8, false);


--
-- Data for Name: silos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.silos VALUES (1, 'Silo Prueba', false);
INSERT INTO public.silos VALUES (2, 'otro de prueba', false);


--
-- Data for Name: tipo_tercero_articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tipo_tercero_articulos VALUES (1, 'Proveedor');
INSERT INTO public.tipo_tercero_articulos VALUES (2, 'Cliente');


--
-- Data for Name: tercero_articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tercero_articulos VALUES (2, 'TRANSPORTE Y SERVICIOS CRISTO DE JOSE, C.A', 'J-402816570         ', '04243408034', '04243408034', 1, false, '');
INSERT INTO public.tercero_articulos VALUES (3, 'NHF', '                    ', '', '', 2, false, '20448338');


--
-- Name: articulos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.articulos_id_seq', 1, true);


--
-- Name: asociaciones_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.asociaciones_id_seq', 4, true);


--
-- Name: conductores_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.conductores_id_seq', 3, true);


--
-- Name: destino_natural_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.destino_natural_id_seq', 1, true);


--
-- Name: destinos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.destinos_id_seq', 2, true);


--
-- Name: estatus_despacho_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.estatus_despacho_id_seq', 7, true);


--
-- Name: estatus_recepcion_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.estatus_recepcion_id_seq', 1, false);


--
-- Name: item_analisis_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.item_analisis_id_seq', 15, true);


--
-- Name: item_producto_analisis_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.item_producto_analisis_id_seq', 1, false);


--
-- Name: perfil_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.perfil_id_seq', 1, false);


--
-- Name: producor_juridico_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.producor_juridico_id_seq', 1, true);


--
-- Name: producto_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.producto_id_seq', 2, true);


--
-- Name: productor_natural_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.productor_natural_id_seq', 3, true);


--
-- Name: silos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.silos_id_seq', 2, true);


--
-- Name: tercero_articulos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tercero_articulos_id_seq', 3, true);


--
-- Name: tipo_tercero_articulos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tipo_tercero_articulos_id_seq', 2, true);


--
-- Name: transporte_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.transporte_id_seq', 1, true);


--
-- Name: unidad_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.unidad_id_seq', 1, true);


--
-- Name: usuarios_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.usuarios_id_seq', 14, true);


--
-- PostgreSQL database dump complete
--

\unrestrict MnjuDMftlgqP6LiNyXIf1M3Hfum4seXEtKEpnglPO5sfiDjtAXr03QS8uxOhGQ7

