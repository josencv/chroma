using System;
using System.Collections.Generic;

namespace Chroma.Infrastructure.Menu
{
    public class MenuSystem
    {
        private Stack<MenuScreen> activeMenuStack;

        public MenuSystem()
        {
            activeMenuStack = new Stack<MenuScreen>();
        }

        public void ShowMenu(MenuScreen menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException("menu");
            }

            if (IsMenuAlreadyInStack(menu))
            {
                throw new ApplicationException("the given menu screen is already in the active stack");
            }

            if(activeMenuStack.Count > 0)
            {
                activeMenuStack.Peek().gameObject.SetActive(false);
            }

            menu.gameObject.SetActive(true);
            activeMenuStack.Push(menu);
        }

        public void CloseCurrentMenu()
        {
            if (activeMenuStack.Count == 0)
            {
                throw new ApplicationException("there is no menu open");
            }

            MenuScreen menu = activeMenuStack.Pop();
            menu.gameObject.SetActive(false);
        }

        private bool IsMenuAlreadyInStack(MenuScreen menu)
        {
            foreach(MenuScreen activeMenu in activeMenuStack)
            {
                if (activeMenu.name == menu.name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
