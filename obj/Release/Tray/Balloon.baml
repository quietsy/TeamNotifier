   M S B A M L   `   `   `  ���� G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamNotifierTeamNotifier      TeamNotifier.Balloon   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ,localclr-namespace:TeamNotifier     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     >�Name$  me��$��45��$	��300�� � 	Resources    FadeInAndOut g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value��%�   &	        �� .��U� $ 00:00:00X�$
 grid��$ (UIElement.Opacity)�.\��� $ 00:00:00��$ 0Q��� $ 00:00:01��$ 0.895Q��� $ 00:00:10��$ 0.895Q��� $ 00:00:11.6000000��$ 0Q� � Triggers :�  :� RoutedEvent$&  local:TaskbarIcon.BalloonShowing��.[��� -   	 ��Name$"	 FadeInAndOut_BeginStoryboard��#��[  .��� -   
 �Name$

 grid��.+��� -    ��Name$ border�� �� CornerRadius$ 10,10,10,10��
���   �$��1q�$��0,0,5,5q�����  �� Color$ 
WhiteSmoke�� �� Opacity$	 0.9Q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  �� $��
10,10,15,0q�$	��Top=�$��14(�$
��Bold"�$
��Wrap=�$��Center=��� $)�#Team Notifier is minimized to tray!��