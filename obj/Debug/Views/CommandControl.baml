   M S B A M L   `   `   `  ����G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TeamNotifier.ViewsTeamNotifier.Views  5      A%clr-namespace:TeamNotifier.ViewModelsTeamNotifier.ViewModels  5      9!clr-namespace:TeamNotifier.ModelsTeamNotifier.Models  5      +clr-namespace:TeamNotifierTeamNotifier  5	      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5
      '    !TeamNotifier.Views.CommandControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1nsvi clr-namespace:TeamNotifier.Views     5      6nsvm%clr-namespace:TeamNotifier.ViewModels     5      1nsm!clr-namespace:TeamNotifier.Models     5      )nsclr-namespace:TeamNotifier     5	      4s&clr-namespace:System;assembly=mscorlib     5
      $��27��5        � 	Resources  5      '   !TeamNotifier.Views.DelayConverter   DelayConverter  MessageColors S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName��%�   &	        &	        5   
   6   S� 5   
   $ 	GetValues��63   "  5       5      M� 5      $ nsm:MessageColors��6   5      5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��4* �6   �� 5      $��4* �6   �� 5      $��9* �6   �� 5      $��3* �6   �� 5      $��2* �6   �� 5      $��3* �6   �� 5      $��2* �6   �� 5      $��2* �6   �� 5       $��1* �6   .+�5"   
   ��  �� ContentStringFormat$ Command {0}��68   $��0��6Z   ��6   �� *Model.CommandNumber+6Z   �� 5#   
   -   5         ��Name$ HotkeyTextBox��5$       ^� ShowOnDisabled . $��Center=�5#   U   $��1��5$   �   $��2,2,2,2q�6�   ��5#      �� *Model.Hotkey+.��5$   �   ��5%      �� 5&      	 �� 
TargetType"	 ��6   .�5'      �� 
 �� Property
 �@�ToolTip �� Value$- 'Select a hotkey to trigger the command.��60    �� Triggers 5(      ^� 5)       ^� 
Conditions 5*      �� 5+   "    �� Binding 6,   �� *DataContext.Model.Room+ �� RelativeSource �� *FindAncestor+ �� AncestorType,   &TeamNotifier.Views.TeamNotifierControl"   �� Value  6�   �� 5,   "    6,   �� *Model.IsLocal+	 False6N   .0�5.      �� 
 � {� .  6B   �� 5/      
 �@�ToolTip$/ )Type a Room to enable non-local commands.��6@   50      52      .��53      54      �� 55   
   -   5         . $
��Left=�55   V   $��2��56      $��2,2,2,2q�6"    �� 	MaxLength$ 40��63   ��55      �� *Model.Message+.��56   �   ��57      �� 58      "	 ��6    59      ^� 5:       5;      �� 5<   "    6,   �� *DataContext.Model.Room+ �� *FindAncestor+"    6�   �� 5=   "    6,   �� *Model.IsLocal+	 False6N   .0�5?      �� 
 � {� .  6B   �� 5@      
 �@�ToolTip$/ )Type a Room to enable non-local commands.��6@   5A      5C      .��5D      ��5E      �� 5F       � ColorInterpolationMode$ ScRgbLinearInterpolation=�6&    �� Opacity$	 0.8Q�6X    �� 
StartPoint$	 0,0&�6g    �� EndPoint$	 1,1&�6x   .��5G      �  � Color$ White��6#    � Offset$ 0Q�61   � 5H      $ White��6#   $	 0.5Q�61   � 5I      $ 1Q�6o    6#   ��  �� ElementName$ ColorComboBox�� �� FallbackValue$ White�� �� Path$
 Text�6o   5J      .��5L   "   5M      �� 5N   
   -   5         ��Name$ ColorComboBox��5N       . $��3��6|    �� Text$ White��6�   $��2,2,2,2q�5O       �� SelectedValue 5N   *   �� *Model.MessageColor+��5O   $   ��  �� Source# [ ��5P      �� 5Q      "	 ��6    5R      ^� 5S       5T      �� 5U   "    6,   �� *DataContext.Model.Room+ �� *FindAncestor+"    6�   �� 5V   "    6,   �� *Model.IsLocal+	 False6N   .0�5X      �� 
 � {� .  6C   �� 5Y      
 �@�ToolTip$/ )Type a Room to enable non-local commands.��6A   5Z      5\      5^      �� 5`   
     ^� ShowDuration$  10000��6   ! � ToolTip$l! fLocal messages will only be seen by you, non-local 
messages will be seen by users with the same Room.��67   $��Center=�5a   7   $��Center=�5b      $��4��60   " d� 	IsChecked" 5a      �� *Model.IsLocal+5b   0       TeamNotifier.MaskedTextBox  5d   
   -   5        #  Name$# _maskedTextBox��5d          TeamNotifier.MaskedTextBox$   KeyDown$$ TimeReminderKeyDown��5g   @   %   GotFocus$% TimeReminderGotFocus��6^   &   LostKeyboardFocus$& TimeReminderLostFocus��6~   $��Center=�5e      $��Center=�6?   '   Mask$' 00:00:00��5f      $��5��6.   $��2q�6>    . $   TeamNotifier.ValueRangeTextBox(   ValueDataType"( ��5g      ��5d   3   �� *Model.TimeReminder+.��5g   �   ��5h      �� 5i      "	 ��6   .�5j      �� 
 �@�ToolTip$3 -The local message will be sent after a delay.��60    5k      ^� 5l       5m      �� 5n   "    6,   �� *DataContext.Model.Room+ �� *FindAncestor+"    6�   �� 5o   "    6,   �� *Model.IsLocal+	 False6N   .0�5q      �� 
 � {� .  6B   �� 5r      
 �@�ToolTip$/ )Type a Room to enable non-local commands.��6@   5s      5u      .��5v      5w      �� 5y   
   $  10000��6   $I! CSingle will show the remaining time when pressing the hotkey again.��5z       . $��Center=�5{   8   $��Center=�6S   $��6��6p   " 6   �� *Model.IsSingle+��5|      �� 5}      "	 ��6    5~      �� 5      ) �� Binding) 6&   �� *Model.TimeReminder+* �� 	Converter#* [  + �� Value	+ False6x   .a�5�      �� 
 � {� .  6C   5�      �� 5�      ) 6&   �� *Model.IsRepeat++ True6I   .a�5�      �� 
 � {� .  6C   5�      5�      5�      �� 5�   
    . $��Center=�5�   8   $��Center=�6S   $��7��6p   " 6   �� *Model.IsRepeat+��5�      �� 5�      "	 ��6   .�5�      �� 
 �@�ToolTip$I CSingle will show the remaining time when pressing the hotkey again.��60    5�      �� 5�      ) 6&   �� *Model.TimeReminder+#* [  	+ False6x   .a�5�      �� 
 � {� .  6C   �� 5�      
 �@�ToolTip$2 ,Only delayed local commands can be repeated.��6A   5�      �� 5�      ) 6&   �� *Model.IsLocal+	+ False6H   .a�5�      �� 
 � {� .  6C   �� 5�      
 �@�ToolTip$2 ,Only delayed local commands can be repeated.��6A   5�      5�      5�      �� 5�   
   $! Delete the command.��6   $��8��6/   
������ $��0q�6X   $��23��6l   $��23��6w   $��Center=�6�   $��Center=�6�   ��6�   �� $ DeleteCommand���6�   �� $ Model.CommandNumber�.��5�      F� +����    @KL �@KL     �@KL @KL �        	$��5��66   , �� Stretch$
, Fill=�6J   
���  ��6Y   5�      5�      5�      