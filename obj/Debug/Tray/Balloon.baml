   M S B A M L   `   `   `  ����G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TeamNotifierTeamNotifier  5          TeamNotifier.Balloon   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ,localclr-namespace:TeamNotifier     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5	      -   5          >�Name$  me��5      $��45��5      $	��300��5       � 	Resources 5         FadeInAndOut g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value��%y  &	        �� 5   
   .��5      U� $ 00:00:00X�5      $
 grid��5      $ (UIElement.Opacity)�5      .\�5      �� $ 00:00:00��5      $ 0Q�5      �� 5      $ 00:00:01��5      $ 0.895Q�5      �� 5      $ 00:00:10��5      $ 0.895Q�5      �� 5      $ 00:00:11.6000000��5      $ 0Q�5      5      5       5!       � Triggers 5"      :� 5#   
    :� RoutedEvent$&  local:TaskbarIcon.BalloonShowing��5$      .[�5%      �� -   5        	 ��Name$"	 FadeInAndOut_BeginStoryboard��5'      #��[  5&      5'      5(      .��5*      � -   5        
 �Name$

 grid��5+   	   .+�5-   
   �� -   5         ��Name$ border��5.       �� CornerRadius$ 10,10,10,10��5/      
���   �$��1q�50   !   $��0,0,5,5q�51      ��52      �� 53       �� Color$ 
WhiteSmoke��6"    �� Opacity$	 0.9Q�65    J� Effect 55      3  -System.Windows.Media.Effects.DropShadowEffect  56      58      �� 59   
   $��
10,10,15,0q�5:      $	��Top=�5;      $��14(�5<      $
��Bold"�5=      $
��Wrap=�5>      $��Center=�5?      �� 5@      $)�#Team Notifier is minimized to tray!��6   5A      5B      5C      