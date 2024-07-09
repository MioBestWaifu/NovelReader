using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Mio.Reader.Utilitarians;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Reader.Components
{
    internal class ShortcutResponder
    {
        public NavigationManager Navigator { get; set; }

        public bool HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.CtrlKey && e.ShiftKey)
            {
                switch (e.Code)
                {
                    case Keys.S:
                        Navigator.NavigateTo("/settings");
                        return true;
                }
            }

            return false;
        }
    }
}
