   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   $	��450��65   $	��450��6E   $	��800��6U   $	��800��6d   .��5	      � -   5         �Name$ MainGrid��5	       � RowDefinitions 5
   
   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5      .+�5   
   �� -   5        $��LoginTB��5      $��0��6!   $	��500��6.   $��10q�6:   $��25(�6F   $��Center=�6T   �� 5   
   $��	UserName:��6   $��0��6$   $
��Left=�61   $��26(�6L   $��Center=�6Z   �� 5   
   -   5        $��
PasswordTB��5      $��1��6$   $	��500��61   $��10q�6=   $��25(�6I   $��Center=�6W   �� 5   
   $��	Password:��6   $��1��6$   $
��Left=�61   $��26(�6L   $��Center=�6Z   � 5   
   -   5        $ Place1��5      $��2��6   6   	� 5   
   -   5         	�Name$ LoginRB��5       d� 	IsChecked$
 TrueY�6'   $	��150��68   $��5q�6D   $��Login��6P   $��30(�6`   $��4��6n   $
��Left=�6{   6�   	� 5   
   -   5        $ SignUpRB��5      $	��150��6(   $��	160,5,0,5q�64   $��Sign Up��6H   $��30(�6Z   $��4��6h   $
��Left=�6u   6�   �� 5   
   -   5         ��Name$ 
ButtonNext��5      $
��Next��6%   $��30(�64   $	��100��6B   $��5q�6N   $��5��6Y   $��Right=�6f   6�   5       5!      