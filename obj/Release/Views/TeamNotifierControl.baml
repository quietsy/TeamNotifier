   M S B A M L   `   `   `  ���� G  CTeamNotifier, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TeamNotifier.ViewsTeamNotifier.Views  A%clr-namespace:TeamNotifier.ViewModelsTeamNotifier.ViewModels  ,    &TeamNotifier.Views.TeamNotifierControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     1nsvi clr-namespace:TeamNotifier.Views     6nsvm%clr-namespace:TeamNotifier.ViewModels     -     � DataContext  3   -TeamNotifier.ViewModels.TeamNotifierViewModel   � 	Resources  �� 
TargetType �� Property �� Value0   *TeamNotifier.Views.PathToFilenameConverter   PathToFilenameConverter��%�  '��       '�� q     '�� �     &	  S    �� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��$
 Bold"���  � ��
 �   ��� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��
 �������  � ��
 �   ��� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��
 �������  � ��
 �   �  .��� .+��� $��Team Notifier��$��8q�$��30(�$	��Top=��� $��Right=�$��	0,25,10,0q�$	��Top=�$
��Bold"�$��14(����� " �� �� Triggers ��  �� Binding ��  �� Path$ IsServerConnected� �� Value True.a���  � ��$ 	Connected����  � ��
 � � ���  �� $ IsServerConnected�	 False.a���  � ��$ Connecting...����  � ��
 �  ����  �� $ 
Model.Room�  .a���  � ��$ Offline Mode����  � ��
 �   ��� $��Right=�$��	0,40,10,0q�$	��Top=�$��12(����� " �� ��  �� $ 
Model.Room�  .a���  � ��$ Type a room to connect����  � ��
 �   �� $��	11,30,0,0q�.+��� $��Profile��$
��Left=�$	��Top=��� -   	 ��Name$	 ProfilesComboBox��$
��Left=�$	��Top=�$��80��$��52,3,0,0q�
 �� SelectedValuePath$

 Name��$ �� IsSynchronizedWithCurrentItem$
 TrueY� �� SelectedItem �� *SelectedProfile+ �� Mode$ TwoWay=� �� SelectedValue �� *SelectedProfile.Name+���� *
Profiles+��  � ToolTip$ Add a new profile.��$��	138,4,0,0q�$��20��$��20��$
��Left=�$	��Top=�
������ $��0q����� $ AddNewProfile�.��F� +���� �     � � @B �     � �@B  � 	$��4�� �� Stretch$
 Fill=�
��� � ��� $ Delete the profile.��$��	158,5,0,0q�$��18��$��18��$
��Left=�$	��Top=�
������ $��0q����� $ DeleteProfile����� " �� ��  �� *Profiles.Count+ 1.a���  � y�$ Hidden=�.��F� +����    @KL �@KL     �@KL @KL �        	$��4��$
 Fill=�
���  ��� 
������ $��Stretch=�$��180,80,180,280q� � ColumnDefinitions �� $��2* ��� $��3* ��� $��3* ��� $��3* � � RowDefinitions �� $��1* ��� $��1* ��� $��1* �.+��� $
��User��$��0��$��0���� $��1��$��0��$��2q� �� 	MaxLength$ 10������ *
Model.User+�� $
��Room��$��0��$��1���� -    ��Name$ RoomTextBox��$B <Non-local messages will be seen by users with the same Room.��$��1��$��1��$��2q�$ 10������ *
Model.Room+�� $��Auto Delete��$��2��$��0��� $��3��$��0�� �� $��5* ��� $	��12* �.+��� -   $+ %Auto delete messages after # seconds.��$��0��$��0��$��2q�$ 10������ *Model.AutoDelete+�� $��Seconds��$
��Left=�$��Regular"�$��1��$��0���� $��Max Messages��$��2��$��1���� -   $+ %Maximum number of messages displayed.��$
��Left=�$��30��$��3��$��1��$��2q����� *Model.MaxMessages+�� $��
Play Sound��$��0��$��2���� $��1��$��2��$��2q� �� *Model.SoundNotificationType+���� *SoundNotificationTypes+�� $��Scheduled Events��$��2��$��2��� $��3��$��2�� �� $��5* ��� $��1* �.+��� -    �� 
IsReadOnly . $I CA file containing recurring local notification based on a schedule.��$��0��$��0��$��2q�$ 10��{���  �� ElementName$- 'IsScheduledNotificationsEnabledCheckBox��$ 	IsChecked����� *" Model.ScheduledNotificationsFile+$ OneWay=� �� 	Converter# [  �� -    ��Name$- 'IsScheduledNotificationsEnabledCheckBox��$��1��$��0��$��Center=�$��Center=� d� 	IsChecked �� *'%Model.IsScheduledNotificationsEnabled+�� 
����  �$��0,0,0,46q����� *NextTaskDescription+�� $
��Auto=�$	��760��$	��170��$��10,206,10,20q�.���� $	��740��.+��  �� $��4* ��� $��4* ��� $��9* ��� $��3* ��� $��2* ��� $��3* ��� $��2* ��� $��2* ��� $��1* �.+��� $��Hotkey��$��1���� $��Message��$��2���� $��Color��$��3���� $��Local��$��4���� $��Delay��$��5���� $��Single��$��6���� $��Repeat��$��7���� ���� *
Commands+���� .R�'   !TeamNotifier.Views.CommandControl  �� $ Add a new command.��$��0,0,0,10q�$��30��$��30��$��Bottom=�$��Center=�
������ $��0q����� $ AddNewCommand�.��F� +���� �     � � @B �     � �@B  � 	$��5��$
 Fill=�
��� � �