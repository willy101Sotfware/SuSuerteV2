﻿namespace SuSuerteV2.Domain.Enumerables
{
    public enum StateTransaction
    {
        Iniciada = 1,
        Aprobada,
        Cancelada,
        AprobadaErrorDevuelta,
        CanceladaErrorDevuelta,
        AprobadaSinNotificar,
        ErrorServicioTercero
    }
}
