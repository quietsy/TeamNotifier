   M S B A M L   `   `   `  ���� G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamNotifierTeamNotifier      TeamNotifier.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ,localclr-namespace:TeamNotifier       �� ShutdownMode$  OnExplicitShutdown=� �� 	Resources    SysTrayMenu m� Command   TeamNotifier.TaskbarIcon  
NotifyIcon   
IconSource   ToolTipText � ContextMenu   DoubleClickCommand   TrayToolTip �� CornerRadius	 J� Opacity
 � DataContext&    TeamNotifier.NotifyIconViewModel�� %�  &	       &	 <     7   �� .@�m� $
��Exit�� �� *ExitApplicationCommand+  $ /Resources/icon.ico��$> 8Double-click to open Team Notifier, right-click to close��8    �� *ShowWindowCommand+ �� 
�������
���   �$��1q�$ 10��$		 0.9Q�$	��285��$��30��._��� $
��Bold"�$��14(�$0��*Double-click to open, right-click to close��$��Center=�$��Center=�
   