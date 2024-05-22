from music21 import *

us = environment.UserSettings()
us['musicxmlPath'] = 'C:\\Program Files\\MuseScore 4\\bin\\MuseScore4.exe'
us['musescoreDirectPNGPath'] = 'C:\\Program Files\\MuseScore 4\\bin\\MuseScore4.exe'
us['musicxmlPath']



score = converter.parse('C:/Users/Florian/Desktop/Alle meine Entchen/Alle meine Entchen.mxl', format='musicxml')
score.removeByClass(metadata.Metadata)
score.show('musicxml.png')