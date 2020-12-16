# Lesson 1

* Erstelle zunächst aus dieser Visual Studio Solution die Bibliothek EDNA.Learning.Csharp.dll.
* Erzeuge anschließend eine neue Konsolenanwendung und füge einen Verweis auf die erzeugte Bibliothek hinzu.
* Die Bibliothek simuliert die Schnittstelle zu einer Steuerung, mit der Daten von der Steurung gelesen werden können. Die Aufgabe besteht nun darin, eine Verbindung zur Steuerung herzustellen und einen Temperaturwert von der Steuerung auszulesen und in der Konsole auszugeben. Der Termperaturwert wird von der Steuerung in Area 4 auf Byte 12 ausgegeben. Es handelt sich um eine 32 Bit Gleitkommazahl.

## Anforderungen
* Schreibe die Klassse ConsoleLogger, die das Interface EDNA.Learning.CSharp.ILogger so implementiert, dass bei einem Aufruf von `void ILog(string message)` eine neue Zeile in der Konsole mit dem Inhalt `message` geschrieben wird.
* Erzeuge eine Instanz der Klasse EDNA.Learning.CSharp.Lesson1.Plc und übergib ihr eine Instanz von ConsoleLogger
* Stelle eine Verbindung zur Plc her
* Lies den Temperaturwert von der Schnittstelle so lange aus, bis die Esc Taste gedrückt wird und gibt ihn in der Konsole aus. Setze dafür den Cursor vor jedem Schreibvorgang auf den Anfang der Zeile zurück, damit der Temperaturwert immer in dieselbe Zeile geschrieben wird.
* Denk daran, die Verbindung zu trennen, bevor das Programm beendet wird

![Example](example.png)