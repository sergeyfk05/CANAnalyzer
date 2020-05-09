/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/

namespace HamburgerMenu.Events
{
    public delegate void ClickedEventHandler(object sender, ClickedEventArgs e); 
    public delegate void HamburgerMenuClickedEventHandler(object sender, HamburgerMenuClickedEventArgs e);
}
