   M S B A M L   `   `   `  ����  animationDemo0    *animationDemo.ControlFolder.inspectControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5	   
   �� 5
      $	��100 �6   �� 5      $	��500 �6   �� 5      $��60 �6   .+�5   
   � $��0��6   .+�5      �� $��系统控制交互界面��6   $��Center=�6'   $��微软雅黑)�6D   $��50(�6V   
�������6d   5      � 5   
   $��1��6   .+�5      �  � ColumnDefinitions 5      �� 5      $	��250 �6'   �� 5      $	��300 �6'   �� 5      $	��250 �6'   .+�5      � $��0��6     5      �� 5      $	��100 �6(   �� 5      $	��200 �6(   �� 5      $	��200 �6(   .+�5      � $��0��6   .+�5      �� -   5         ��Name$ label1��5       $��录入形式��60   $��Center=�6?   $��Center=�6\   $��25(�6w   
�������6�   5       � 5!      $��1��6   .+�5"      � $��0,0,0,0q�6$    � Orientation$ 
Horizontal=�65    � 
ItemHeight$	 150��6N    � 	ItemWidth$	 250��6_   $��Center=�6o   .+�5#      �� -   5         ��Name$ scannerButton��5#   %   $��扫码枪录入��6<   $��75��6L   $	��220��6X   $��0,0,0,0q�6d   $��25(�6u   6�   5$      5%      � 5&      $��2��6   .+�5'      � $��0,0,0,0q�6$   $ 
Horizontal=�65   $ 75��6N   $	 250��6^   $��Center=�6n   .+�5(      �� -   5        $ 
RFIDButton��5(   %   $��
RFID录入��69   $��75��6J   $	��220��6V   $��0,0,0,0q�6b   $��25(�6s   6�   5)      5*      5+      � 5,      $��1��6     5-      �� 5.      $	��100 �6(   �� 5/      $	��200 �6(   �� 50      $	��200 �6(   .+�52      � $��0��6   .+�53      �� -   5        $ label2��53       $��检测识别��60   $��Center=�6?   $��Center=�6\   $��30(�6w   
�������6�   54      � 55      $��1��6   .+�56      �� $��扫描信息:��6    $
��Left=�60   $��10,10,0,148q�6K   $��Center=�6`   $��25(�6{   
�������6�   �� 57      -   5         ��Name$ 
scannerBox��57   &   $
��Left=�6:   $	��120��6U   $��	10,55,0,0q�6b   $	��Top=�6u   $	��280��6�   $��20(�6�   . �58      /� .T�59   "   J� .*�6,   .*�5:   &   �� �6*   ��  �� Path$ scannerInfo�5;   #   5<      5=      5>      � 5?      $��2��6   .+�5@      �� $��RFID信息:��6    $
��Left=�62   $��10,10,0,148q�6M   $��Center=�6b   $��25(�6}   
�������6�   �� 5A      -   5        $ RFIDBox��5A   &   $
��Left=�67   $	��120��6R   $��	10,55,0,0q�6_   $	��Top=�6r   $	��280��6�   $��20(�6�   . �5B      /� .T�5C   "   J� .*�6,   .*�5D   &   �� �6*   �� $ RFIDInfo�5E   #   5F      5G      5H      5I      � 5J      $��3��6     5K      �� 5L      $	��100 �6(   �� 5M      $	��200 �6(   �� 5N      $	��200 �6(   .+�5P      � $��0��6   .+�5Q      �� -   5        $ label3��5Q       $��存储控制��60   $��Center=�6?   $��Center=�6\   $��25(�6w   
�������6�   5R      � 5S      $��1��6   .+�5T      � $��0,0,0,0q�6$   $ 
Horizontal=�65   $ 75��6N   $	 250��6^   $��Center=�6n   .+�5U      �� -   5        $ startButton��5U   %   $��存储��6:   $��75��6G   $	��220��6S   $��0,0,0,0q�6_   $��30(�6p   5V      5W      5X      � 5Y      $��2��6   .+�5Z      � $��0,0,0,0q�6$   $ 
Horizontal=�65   $	 105��6N   $	 250��6_   $��Center=�6o   .+�5[      �� -	   5        $ 
stopButton��5[   %   $��擦除��69   $	��105��6F   $	��200��6S   $��0,0,0,0q�6_   $��30(�6p   5\      5]      5^      5_      5`      5a      � 5b   
   $��2��6   .+�5c      �� -
   5        $ 	timeLabel��5c      $��	时间：��6'   $��Right=�65   $	��120��6Q   $	��Top=�6^   $��30(�6v   
�������6�   5d      5e      5f      