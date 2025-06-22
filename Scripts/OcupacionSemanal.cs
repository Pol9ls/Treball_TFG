using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[Serializable]
public class OcupacioHora
{

    public int hour;
    public int occupancyPercent;
    
}

public class OcupacioSemanal
{
    public List<OcupacioHora> Su;
    public List<OcupacioHora> Mo;
    public List<OcupacioHora> Tu;
    public List<OcupacioHora> We;
    public List<OcupacioHora> Th;
    public List<OcupacioHora> Fr;
    public List<OcupacioHora> Sa;
}