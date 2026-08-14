using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace principal
{
    public class detalle_articuloBO
    {
      public  int codigo {
            get;
            set;
        }
        public string  descripcion
        {
            get;
            set;
        }

        public decimal Precio
        {
            get;
            set;
        }
        public int entrada
        {
            get;
            set;
        }
        public int salida
        {
            get;
            set;
        }
        public int cantidad
        {
            get;
            set;
        }
        public string unidad {
            get;
            set;
        }
        public string estante
        {
            get;
            set;
        }

        public string seccion
        {
            get;
            set;
        }
        public int id_seccion
        {
            get;
            set;
        }
        public int id_estante
        {
            get;
            set;
        }

        public decimal Total
        {
            get;
            set;
        }
    }
}
