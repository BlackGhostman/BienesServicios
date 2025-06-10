using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(T enumValue) where T : Enum
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description ?? enumValue.ToString();
        }


        private static string GetDescripcionTipoPresupuesto(int tipo)
        {
            if (Enum.IsDefined(typeof(EnumExtensions.TipoPresupuesto), tipo))
            {
                return EnumExtensions.GetDescription((EnumExtensions.TipoPresupuesto)tipo);
            }

            return "Sin asignar";
        }


        // Enumeración para Tipo de documentos adjunto a la Bien y Servicios
        public enum TipoDoc
        {
            [Description("Pliego de condiciones")]
            expediente = 1,
            [Description("Decisión Inicial")]
            orden = 2,
            [Description("Estudio de Mercado")]
            estudiomercado = 3,
            [Description("BS-Firmada")]
            FirmaSolicitante = 4,
            [Description("Otros")]
            Otros = 5,
        }


            public enum EstadoSolicitud
            {

            [Description("Solicitud Creada - Sin Firma")]
            SolicitudCreada = 0,

            [Description("Solicitud Firmada")]
             SolicitudFirmada = 1,

             [Description("Pre-Aprobado-Presupuesto")]
             PreAprobadoPresupuesto = 2,
             [Description("Pre-Rechazada-Presupuesto")]
             PreRechazadaPresupuesto = 3,

            [Description("Aprobada - Presupuesto")]
            AprobadaPresupuesto = 4,
            [Description("Rechazada - Presupuesto")]
             RechazadaPresupuesto = 5,


            [Description("Pre-Aprobado-Financiero")]
            PreAprobadoFinanciero = 6,
            [Description("Pre-Rechazada-Financiero")]
            PreRechazadaFinanciero = 7,
            
            [Description("Aprobada - Financiero")]
            AprobadaFinanciero = 8,
            [Description("Rechazada - Financiero")]
            RechazadaFinanciero = 9,


            [Description("Pre-Aprobado-Proveeduria")]
            PreAprobadoProveeduria = 10,
            [Description("Pre-Rechazada-Proveeduria")]
            PreRechazadaProveeduria = 11,
            
            [Description("Aprobada - Proveeduria")]
            AprobadaProveeduria = 12,
            [Description("Rechazada - Proveeduria")]
            RechazadaProveeduria = 13,

            [Description("Anular - Proveeduria")]
            AnularProveeduria = 14,


            [Description("Aprobada - Alcaldía")]
            AprobadaAlcaldia = 15,
            [Description("Rechazada - Alcaldía")]
            RechazadaAlcaldia = 16,
            
            [Description("Revisión - Alcaldía")]
            RevisionAlcaldia = 17,


            [Description("Anular - Alcaldía")]
            AnularAlcaldia = 18,


            [Description("Anular - Presupuesto")]
            AnularPresupuesto = 19,

            [Description("Anular - Financiero")]
            AnularFinanciero = 20,


            [Description("Anular - Solicitante")]
            AnularSolicitante = 21,


            [Description("Pendiente - Modificacion")]
            PendienteModificacion = 22,


        }


        public enum TipoPresupuesto
        {
            [Description("Ordinario en Ejecución")]
            Ordinario = 1,
            [Description("Extraordinario")]
            Extraordinario = 2,
            [Description("Modificación")]
            Modificación = 3,
        }

        public enum TipoBienServicio
        {
            [Description("Decisión Inicial")]
            DecisiónInicial = 1,
            [Description("Carga de presupuesto o contrato en ejécución")]
            CargaEjecucion = 2,
        }



        public enum Roles
        {
            [Description("Administrador")]
            Administrador = 1,
            [Description("Dirección Presupuesto")]
            DireccionPresupuesto = 2,
            [Description("Soporte Presupuesto")]
            SoportePresupuesto = 3,
            [Description("Dirección Financiera")]
            DireccionFinanciera = 4,
            [Description("Soporte Financiero")]
            SoporteFinanciero = 5,
            [Description("Dirección Proveduría")]
            DireccionProveduria = 6,
            [Description("Soporte-Inicial Proveduría")]
            SoporteInicialProveduria = 7,
            [Description("Soporte-Carga Proveduría")]
            SoporteCargaProveduria = 8,
            [Description("Solicitante")]
            Solicitante = 9,
            [Description("Alcaldía")]
            alcaldia = 10,
        }






        public enum DestinoRechazoPresupuesto
        {
            [Description("Pre-Presupuesto")]
            PrePresupuesto = 1,
            [Description("Solicitante")]
            Solicitante = 2
        }

        public enum DestinoRechazoFinanciero
        {
            [Description("Pre-Presupuesto")]
            PrePresupuesto = 1,
            [Description("Presupuesto")]
            Presupuesto = 2,
            [Description("Solicitante")]
            Solicitante = 3
        }


        public enum DestinoRechazoPreProveeduria
        {
            [Description("Pre-Presupuesto")]
            PrePresupuesto = 1,
            [Description("Presupuesto")]
            Presupuesto = 2,
            [Description("Financiero")]
            Financiero = 3,
            [Description("Alcaldía")]
            Alcaldia = 4,
            [Description("Solicitante")]
            Solicitante = 5

        }


        public enum DestinoRechazoProveeduria
        {
            [Description("Pre-Preproveduría")]
            Preproveduria = 1,
            [Description("Pre-Presupuesto")]
            PrePresupuesto = 2,
            [Description("Presupuesto")]
            Presupuesto = 3,
            [Description("Financiero")]
            Financiero = 4,
            [Description("Alcaldía")]
            Alcaldia = 5,
            [Description("Solicitante")]
            Solicitante = 6
            
        }


    }

}
