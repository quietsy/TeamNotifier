   M S B A M L   `   `   `  ���� G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TeamNotifier.ViewsTeamNotifier.Views  A%clr-namespace:TeamNotifier.ViewModelsTeamNotifier.ViewModels  9!clr-namespace:TeamNotifier.ModelsTeamNotifier.Models  +clr-namespace:TeamNotifierTeamNotifier  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem '    !TeamNotifier.Views.CommandControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     1nsvi clr-namespace:TeamNotifier.Views     6nsvm%clr-namespace:TeamNotifier.ViewModels     1nsm!clr-namespace:TeamNotifier.Models     )nsclr-namespace:TeamNotifier     4s&clr-namespace:System;assembly=mscorlib     $��27��  � 	Resources  '   !TeamNotifier.Views.DelayConverter   DelayConverter  MessageColors S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� TypeName��%T   &	        &	        S� $ 	GetValues��"   M� $ nsm:MessageColors��.���  � ColumnDefinitions �� $��4* ��� $��4* ��� $��9* ��� $��3* ��� $��2* ��� $��3* ��� $��2* ��� $��2* ��� $��1* �.+���  �� ContentStringFormat$ Command {0}��$��0������ *Model.CommandNumber+�� -    ��Name$ HotkeyTextBox�� ^� ShowOnDisabled . $��Center=�$��1��$��2,2,2,2q����� *Model.Hotkey+.������ 	 �� 
TargetType"	 ��.��� 
 �� Property
 �@�ToolTip �� Value$- 'Select a hotkey to trigger the command.�� �� Triggers ^�  ^� 
Conditions ��  �� Binding �� *DataContext.Model.Room+ �� RelativeSource �� *FindAncestor+ �� AncestorType,   &TeamNotifier.Views.TeamNotifierControl"   �� Value  ��  �� *Model.IsLocal+	 False.0��� 
 � {� .  �� 
 �@�ToolTip$/ )Type a Room to enable non-local commands.��.���� -    . $
��Left=�$��2��$��2,2,2,2q� �� 	MaxLength$ 40������ *Model.Message+.������ "	 �� ^�  ��  �� *DataContext.Model.Room+ �� *FindAncestor+"    ��  �� *Model.IsLocal+	 False.0��� 
 � {� .  �� 
 �@�ToolTip$/ )Type a Room to enable non-local commands.��.������  � ColorInterpolationMode$ ScRgbLinearInterpolation=� �� Opacity$	 0.8Q� �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ White�� � Offset$ 0Q�� $ White��$	 0.5Q�� $ 1Q� ��  �� ElementName$ ColorComboBox�� �� FallbackValue$ White�� �� Path$
 Text�.���� -    ��Name$ ColorComboBox�� . $��3�� �� Text$ White��$��2,2,2,2q� �� SelectedValue �� *Model.MessageColor+����  �� Source# [ ���� "	 �� ^�  ��  �� *DataContext.Model.Room+ �� *FindAncestor+"    ��  �� *Model.IsLocal+	 False.0��� 
 � {� .  �� 
 �@�ToolTip$/ )Type a Room to enable non-local commands.����   ^� ShowDuration$  10000��! � ToolTip$l! fLocal messages will only be seen by you, non-local 
messages will be seen by users with the same Room.��$��Center=�$��Center=�$��4��" d� 	IsChecked" �� *Model.IsLocal+    TeamNotifier.MaskedTextBox  -   #  Name$# _maskedTextBox��    TeamNotifier.MaskedTextBox$   KeyDown$$ TimeReminderKeyDown��%   GotFocus$% TimeReminderGotFocus��&   LostKeyboardFocus$& TimeReminderLostFocus��$��Center=�$��Center=�'   Mask$' 00:00:00��$��5��$��2q� . $   TeamNotifier.ValueRangeTextBox(   ValueDataType"( ������ *Model.TimeReminder+.������ "	 ��.��� 
 �@�ToolTip$3 -The local message will be sent after a delay.�� ^�  ��  �� *DataContext.Model.Room+ �� *FindAncestor+"    ��  �� *Model.IsLocal+	 False.0��� 
 � {� .  �� 
 �@�ToolTip$/ )Type a Room to enable non-local commands.��.���� $  10000��$I! CSingle will show the remaining time when pressing the hotkey again.�� . $��Center=�$��Center=�$��6��" �� *Model.IsSingle+���� "	 �� �� ) �� Binding) �� *Model.TimeReminder+* �� 	Converter#* [  + �� Value	+ False.a��� 
 � {� .  �� ) �� *Model.IsRepeat++ True.a��� 
 � {� .  ��  . $��Center=�$��Center=�$��7��" �� *Model.IsRepeat+���� "	 ��.��� 
 �@�ToolTip$I CSingle will show the remaining time when pressing the hotkey again.�� �� ) �� *Model.TimeReminder+#* [  	+ False.a��� 
 � {� .  �� 
 �@�ToolTip$2 ,Only delayed local commands can be repeated.���� ) �� *Model.IsLocal+	+ False.a��� 
 � {� .  �� 
 �@�ToolTip$2 ,Only delayed local commands can be repeated.���� $! Delete the command.��$��8��
������ $��0q�$��23��$��23��$��Center=�$��Center=����� $ DeleteCommand����� $ Model.CommandNumber�.��F� +����    @KL �@KL     �@KL @KL �        	$��5��, �� Stretch$
, Fill=�
���  ��