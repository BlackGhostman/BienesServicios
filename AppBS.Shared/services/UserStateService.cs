using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBS.Shared.services
{
    public class UserStateService
    {
        private Usuario? _usuarioAutenticado;

        // Constructor que inicializa el estado
        public UserStateService()
        {
            _usuarioAutenticado = new Usuario { Rol = 0 }; // Inicializa con un valor predeterminado
        }

        public Usuario? UsuarioAutenticado
        {
            get => _usuarioAutenticado;
            private set
            {
                if (_usuarioAutenticado != value)
                {
                    _usuarioAutenticado = value;
                    NotifyStateChanged();
                }
            }
        }

        public event Action? OnChange;

        public void SetUsuario(Usuario usuario)
        {
            UsuarioAutenticado = usuario;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }


}
