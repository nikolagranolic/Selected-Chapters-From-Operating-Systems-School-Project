   M S B A M L   `   `   `  ����>  :GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GUIGUI  5   	       GUI.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:GUI     5   	     � Title$  Scheduling options��5   	   $	��264��6$   $	��400��61    � 
ResizeMode$ NoResize=�6=   .��5	      � $��10,10,10,10q�6    � ColumnDefinitions 5
   
   �� 5      $
��546* �6   �� 5      $	��73* �6   .+�5   
   �� -   5         ��Name$ SchedulingOptionsComboBox��5      $
��Left=�66   $��22��6Q   $��	82,44,0,0q�6]   $	��Top=�6p   $	��216��6�    �� SelectedIndex$ 0��6�   .@�5      �� $
��FIFO��6   �� 5      $��Priority��6   �� 5      $��Time Slicing��6   �� 5      $��
Preemptive��6   5      �� 5   
   $��Scheduling options:��6   $
��Left=�6.   $��29��6I   $��	82,15,0,0q�6U   $	��Top=�6h   $	��136��6�   �� 5   
   $&�� Max. number of concurrent tasks:��6   $
��Left=�6;   $��	82,71,0,0q�6V   $	��Top=�6i   �� 5   
   -   5         ��Name$" MaxNoOfConcurrentTasksSlider��5      $
��Left=�67   $��
82,102,0,0q�6R   $	��Top=�6f   $	��216��6~    � SmallChange$ 1Q�6�    � Maximum$ 20Q�6�    � Value$ 1Q�6�   	 �� TickPlacement$	 TopLeft=�6�   
 � Minimum$
 1Q�6�    �� AutoToolTipPlacement$ TopLeft=�6�   �� 5   
   -   5         ��Name$ CreateSchedulerButton��5      $��OK��60   $
��Left=�6a   $��158,156,0,0q�6|   $	��Top=�6�    J� RenderTransformOrigin$ -1.413,-1.777&�6�   $��28��6�   $��64��6�   5      5      