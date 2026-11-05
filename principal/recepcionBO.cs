using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace principal
{
    public class recepcionBO
    {
        public int id {get;set;}
        public decimal factorMema { get; set; }

      
        public string nro_placa { get; set; }
        public int id_conductor { get; set; }
        public DateTime fecha { get; set; }
        public string nro_guia { get; set; }
        public decimal peso_guia { get; set; }
        public int id_producto { get; set; }
        public int id_productor { get; set; }
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
        public int id_productor_juridico { get; set; }

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
        public string usuario { get; set; }

        public string humedad { get; set; }
        public string impureza { get; set; }
        public string gdt { get; set; }
         public string nombresilo { get; set; }

        /// <summary>Precio tonelada $ a BCV (tasa del día/producto).</summary>
        public decimal precio_tonelada_usd { get; set; }
        /// <summary>Precio tonelada en $ efectivo (tasa del día/producto).</summary>
        public decimal precio_efectivo_usd { get; set; }
        /// <summary>Precio tonelada $ para modalidad 50/50 (mitad efectivo).</summary>
        public decimal precio_50_50_usd { get; set; }
        /// <summary>Valor del dólar en Bs (tasa del día/producto).</summary>
        public decimal tasa_dolar_bs { get; set; }
        /// <summary>Texto de precios/tasas visibles en boleta según modalidad.</summary>
        public string detalle_precios_boleta { get; set; }
        /// <summary>Modalidad de pago: BS/BCV, 50/50 o Efectivo $.</summary>
        public string modalidad_pago { get; set; }
        /// <summary>Tasa especial asociada a esta recepción (tasa_diaria.id); 0 = usar tasa del día.</summary>
        public int id_tasa { get; set; }
        /// <summary>Importe en $ efectivo según modalidad (en 50/50 = mitad del peso).</summary>
        public decimal total_usd { get; set; }
        /// <summary>Importe en Bs BCV según modalidad (en 50/50 = mitad del peso).</summary>
        public decimal total_bs { get; set; }
       
        public List<analisis> listaAnalisis = new List<analisis>();
        
    }
}
