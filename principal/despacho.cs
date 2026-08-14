using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace principal
{
    public class despacho
    {

        public int id { get; set; }
        public decimal factorMema { get; set; }


        public string nro_placa { get; set; }
        public int id_conductor { get; set; }
        public DateTime fecha { get; set; }
        public string nro_guia { get; set; }
        public decimal peso_guia { get; set; }
        public int id_producto { get; set; }
        public int id_destino { get; set; }
        public string observaciones { get; set; }
        public int id_estatus { get; set; }
        public decimal peso_bruto { get; set; }
        public decimal peso_tara { get; set; }
        public decimal peso_neto { get; set; }
        public decimal peso_acondicionado { get; set; }
        public decimal peso_merma { get; set; }
        public int id_silos { get; set; }
        public DateTime hora_entrada { get; set; }
        public DateTime hora_salida { get; set; }
        public int id_destino_natural { get; set; }

        public int cedulaconductor { get; set; }
        public int cedulaproductor { get; set; }
        public string nombreconductor { get; set; }
        public string nombreproductornatural { get; set; }

        public string rif { get; set; }

        public string nombreproductorjuridico { get; set; }
        public string letranatural { get; set; }
        public string letrajuridica { get; set; }
        public string empresa { get; set; }
        public string direccion_empresa { get; set; }
        public string nombre_producto { get; set; }
        public string nombre_asociacion { get; set; }
        public string nombre_trasnporte { get; set; }
        public bool resumen { get; set; }

        public string usuario{ get; set; }
        public string humedad { get; set; }
        public string impureza { get; set; }
        public string gdt { get; set; }
        public string nombresilo { get; set; }


        public List<analisis> listaAnalisis = new List<analisis>();
    }
}
