/* ============================================
            Uduino NeoPixel Project.
  FastLED library: https://github.com/FastLED/
  =============================================== */
  
#include "FastLED.h"
#include "Uduino.h"
#include "Wire.h" // For I2C
//#include "LCD.h" // For LCD
#include <LiquidCrystal_I2C.h>
#define NUM_LEDS 40
#define DATA_PIN 6

LiquidCrystal_I2C lcd(0x27,16,2);
CRGB leds[NUM_LEDS];

Uduino uduino("pixels");
uint8_t currentNeopixelMode = 0; // 0 = Manually set led

void setup() {
  Serial.begin(115200);
  delay(1000); // FastLedSanityCheck
  FastLED.addLeds<WS2812B, DATA_PIN, RGB>(leds, NUM_LEDS);
  uduino.addCommand("SetPixel", SetPixel);
  uduino.addCommand("startLCD", startLCD);
  uduino.addCommand("changeText", changeText);
}

void startLCD()
{
    lcd.init();
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Gear: 1");
  lcd.setCursor(0, 1);
  lcd.print("Time left: N/A");
}

void changeText()
{
  char *arg;
  arg = uduino.next();
  if((int)atoi(arg) >= 1 && (int)atoi(arg) <= 5)
  {
    lcd.setCursor(0, 0);
    lcd.print("Gear: ");
    lcd.print(arg);
  }
  else
  {
    lcd.setCursor(0, 1);
    lcd.print("Time left: ");
    lcd.print(arg);
  }
}

void SetPixel() {
  char *arg;
  arg = uduino.next();
  int led =  atoi(arg);

  arg = uduino.next();
  int g = atoi(arg);

  arg = uduino.next();
  int r = (int)atoi(arg);

  arg = uduino.next();
  int b = (int)atoi(arg);
  
   leds[led].setRGB(r,g,b);
   FastLED.show();
   //lcd.print("Hi");
}




void SetLuminosity(uint8_t brightness) {
  // Move a single white led 
   for(int led = 0; led < NUM_LEDS; led = led + 1) {
      // Turn our current led on to white, then show the leds
      leds[led].setRGB( brightness, brightness, brightness);
   }
   FastLED.show();

}
void loop() {

   uduino.readSerial();
   delay(15);
}


void NeoPixelMode(uint8_t brightness) {

}
