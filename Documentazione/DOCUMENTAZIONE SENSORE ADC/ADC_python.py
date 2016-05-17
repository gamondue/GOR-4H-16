


from Adafruit_ADS1x15 import ADS1x15

pga = 6144
sps = 8
adc = ADS1x15(ic=0x01)

dato = ((adc.readADCSingleEnded(0, pga, sps))/1000)   ## legge il valore dal sensore ADC

##print(dato/1000) ## stampa il risultato

datoStr = str(dato)





# Scrive un file.
out_file = open("Valore_ADC.txt","w")
out_file.write(datoStr)
out_file.close()

# Legge un file.
in_file = open("Valore_ADC.txt","r")
text = in_file.read()
in_file.close()

print text,