   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	       GUI.CreateTaskWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	     � Title$  CreateTaskWindow��5   	   $	��561��6"   $	��741��6/    � 
ResizeMode$ NoResize=�6;   .��5	      � $��5 5 5 5q�6    � RowDefinitions 5
   
   �� 5      $
��403* �6   �� 5      $	��10* �6   �� 5      $	��11* �6   .+�5   
   �� $��2,2,2,2q�6    �� Orientation$ Vertical=�6&   $��3��6=   .+�5      �� $ 
Horizontal=�6   $	��416��62   .+�5      �� $��5 0 0 0q�6   $ Vertical=�6.   $	��533��6E   $	��216��6R   .+�5      �� $��0 5 0 0q�6!   $ Vertical=�62   .+�5      �� $	��130��6    $
��Left=�6,   $��Task parameters��6G   �� 5      6&   5      �� 5      $��0 5 0 0q�6!   $ 
Horizontal=�62   .+�5      �� $	��125��6    .��6,   
Priority66   5      �� 5      $��0 5 0 0q�6!   $ 
Horizontal=�62   .+�5      �� -   5         ��Name$ PrioritySlider��5   !   $	��212��69    � SmallChange$ 1Q�6E    � Value$ 1Q�6U    �� TickPlacement$ TopLeft=�6_    � Minimum$ 1Q�6w   	 �� AutoToolTipPlacement$	 TopLeft=�6�   5      ]� 5      $��0 5 0 0q�6    .+�5      �� $	��185��6    .��6,   
Deadline66   �� 5      -   5        
 ��Name$
 DeadlineCheckBox��5   #    d� 	IsChecked$ FalseY�6=   $��5 0 0 0q�6O   $��Center=�6`   $��Center=�6}   6�   �� 5      -   5         ��Name$ DeadlineInvalidInputLabel��5       $y�Hidden=�6C   $��Right=�6W   
���  ��$��4 4 0 0q�6�   .��6�   Invalid input6�   5       ]� 5!      $��0 5 0 0q�6    .+�5"      (  "System.Windows.Controls.DatePicker  -   5          Name$ DeadlineDatePicker��5"   c   $	��200��6   $��5,0,0,0q�6�   $��25.96��6�   $
��Left=�6�   {�6%   �� *	IsChecked+ �� ElementName$ DeadlineCheckBox��6�   5#      ]� 5$      $��0 5 0 0q�6    .+�5%      �� -   5         ��Name$ DeadlineTimePicker��5%   `   $	��200��6|   $��5,0,0,0q�6�   $��HH-mm-ss��6�   $��25.96��6�   $
��Left=�6�   {�6"   �� *	IsChecked+$ DeadlineCheckBox��6�   5&      ]� 5'      $��0 5 0 0q�6    .+�5(      �� $	��186��6    .��6,   Start date and time6A   �� 5)      -   5        $
 StartDateAndTimeCheckBox��5)   #   $ FalseY�6E   $��5 0 0 0q�6W   $��Center=�6h   $��Center=�6�   6�   �� 5*      -   5        $' !StartDateAndTimeInvalidInputLabel��5*       $y�Hidden=�6K   $��Right=�6_   
���  ��$��4 4 0 0q�6�   .��6�   Invalid input6�   5+      ]� 5,      $��0 5 0 0q�6    .+�5-        -   5        $  StartDateAndTimeDatePicker��5-   k   $	��200��6�   $��5,0,0,0q�6�   $��25.96��6�   $
��Left=�6�   {�6%   �� *	IsChecked+$ StartDateAndTimeCheckBox��6�   5.      ]� 5/      $��0 5 0 0q�6    .+�50      �� -	   5        $  StartDateAndTimeTimePicker��50   h   $	��200��6�   $��5,0,0,0q�6�   $��HH-mm-ss��6�   $��25.96��6�   $
��Left=�6�   {�6"   �� *	IsChecked+$ StartDateAndTimeCheckBox��6�   51      ]� 52      $��0 5 0 0q�6    .+�53      �� $	��175��6    .��6,   Allowed execution time6D   �� 54      -
   5        $"
 AllowedExecutionTimeCheckBox��54   #   $ FalseY�6I   $��5 0 0 0q�6[   $��Center=�6l   $��Center=�6�   6�   55      ]� 56      $��0 5 0 0q�6    .+�57      �� -   5        $! AllowedExecutionTimeTextBox��57   l   $	��200��6�   $��5,0,0,0q�6�   $��25.96��6�   $
��Left=�6�   {�6"   �� *	IsChecked+$" AllowedExecutionTimeCheckBox��6�   58      59      �� 5:      -   5         ��Name$" TaskSpecificConfigStackPanel��5:      $��24 5 0 0q�6C   $ Vertical=�6U   $	��357��6l   .+�5;      �� $ Vertical=�6!   .+�5<      ]� .+�5=      �� $	��130��6$   $
��Left=�60   $��Task-specific config��6K   �� 5>      -   5        $- 'UserTaskSpecificConfigInvalidInputLabel��5>   $   $y�Hidden=�6U   $��Right=�6i   
���  ��.��6�   Invalid input6�   5?      �� 5@      6&   �� 5A      $ 
Horizontal=�6%   .+�5B      �� $	��130��6$   $��	Task type��60   5C      �� 5D      $ 
Horizontal=�6%   .+�5E      �� -   5         ��Name$ TaskTypeListBox��5E   &   $	��400��6?   $
��Auto=�6N   $
��Bold"�6~   $��25.96��6�   6�   5F      �� 5G      -   5        $ UserTaskJsonTextBox��5G   "   $��0 5 0 0q�6?   $	��400��6P    �� AcceptsReturn$
 True��6_    �� 
AcceptsTab$
 True��6t   $
��Auto=�6�   $	��210��6�   5H      5I      5J      �� 5K      $��0 12 0 0q�6   5L      �� 5M      $��73��6   .+�5N      �� -   5        $ ErrorMessageLabel��5N   ,   $y�Hidden=�6   
���  ��$��25.96��6X   $��Center=�6g   .��6�   31The user job-specific configuration is malformed.6�   �� 5O      $��Center=�6   $��0 5 0 0q�6:   $ 
Horizontal=�6K   .+�5P      �� -   5         ��Name$ FinishButton��5P      $��88��63   $��32��6>   ���	#FFDDDDDD$��Finish��6a   6r   �� 5Q      -   5        $ CancelButton��5Q      $��12 0 0 0q�63   $��88��6E   $��32��6P   $��Cancel��6\   6m   5R      5S      5T      5U      5V      