﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdministracionDeUsuarios;

namespace General.MAU
{
    public interface IRepositorioDePermisosSobreAreas
    {
        List<Area> AreasAdministradasPor(Usuario usuario);
        Area AsignarAreaAUnUsuario(Usuario usuario, Area area);
    }
}