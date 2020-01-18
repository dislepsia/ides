using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cnrl.Models
{
    public class Cuota
    {
        public long Id { get; set; }
        public int CodPlan { get; set; }
        [DisplayName("Alumno")]
        public long Dni { get; set; }
        [DisplayName("Cuota N°")]
        public int NroCuota { get; set; }
        [DisplayName("Cantidad de Cuotas")]
        public int TotalCuota { get; set; }
        [DisplayName("Importe 1er Vencimiento")]
        public decimal Importe { get; set; }
        public DateTime? _fechavto;
        public string Periodo { get; set; }


        public string Nombre { get; set; }
        public string Apellido { get; set; }

        [DisplayName("Fecha 1er Vencimiento")]
        public DateTime? fechavto
        {
            get
            {
                if (_fechavto == new DateTime(1900, 1, 1))
                {
                    return null;
                }
                return _fechavto;
            }
            set { _fechavto = value; }
        }
        [DisplayName("Importe 2do Vencimiento")]
        public decimal Importe2 { get; set; }
        public DateTime? _fechavto2;
        [DisplayName("Fecha 2do Vencimiento")]
        public DateTime? fechavto2
        {
            get
            {
                if (_fechavto2 == new DateTime(1900, 1, 1))
                {
                    return null;
                }
                return _fechavto2;
            }
            set { _fechavto2 = value; }
        }
        public DateTime? _fechaPago;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fechaPago
        {
            get
            {
                if (_fechaPago == new DateTime(1900, 1, 1))
                {
                    return null;
                }
                return _fechaPago;
            }
            set { _fechaPago = value; }
        }
        public int NroRec { get; set; }
        public string Estado;
        public string EstadoDescripcion
        {
            get
            {
                if (Estado == "0")
                {
                    return "Impaga";
                }
                if (Estado == "P")
                {
                    return "Paga";
                }
                if (Estado == "B")
                {
                    return "Baja";
                }

                return Estado;
            }
            set { Estado = value; }
        }
        public DateTime? _fechaBaja;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fechaBaja
        {
            get {
                if (_fechaBaja == new DateTime(1900, 1, 1))
                {
                    return  null;
                }
                return _fechaBaja; }
            set { _fechaBaja = value; }
        }
        public String Motivo { get; set; }
        public int CodCon { get; set; }
        public string Concepto { get; set; }
        [DisplayName("Oferta")]
        public int NroComision { get; set; }
        public int NroCurso { get; set; }
        public string Curso { get; set; }
        public string Origen { get; set; }
        [DisplayName("N° Factura")]
        public string NroFactura { get; set; }
        public string Usuario { get; set; }
        
    }
}