   M S B A M L   `   `   `  ����G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamNotifierTeamNotifier  5          TeamNotifier.App   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,localclr-namespace:TeamNotifier     5        �� ShutdownMode$  OnExplicitShutdown=�5       �� 	Resources 5         SysTrayMenu m� Command   TeamNotifier.TaskbarIcon  
NotifyIcon   
IconSource   ToolTipText � ContextMenu   DoubleClickCommand   TrayToolTip �� CornerRadius	 J� Opacity
 � DataContext&    TeamNotifier.NotifyIconViewModel�� 5   
   %m  &	       &	 a     7   �� 5
      .@�5      m� $
��Exit��6    6)   �� *ExitApplicationCommand+5        5      $ /Resources/icon.ico��5      $> 8Double-click to open Team Notifier, right-click to close��5      8    5      �� *ShowWindowCommand+ 5      �� 5      
�������
���   �$��1q�5      $ 10��5      $		 0.9Q�5      $	��285��5      $��30��5      ._�5      �� $
��Bold"�6$   $��14(�66   $0��*Double-click to open, right-click to close��5      $��Center=�5       $��Center=�5!      5"      
 5%        5&      5(      5*      5-      