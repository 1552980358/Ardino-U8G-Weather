#include <Arduino.h>
#include <U8glib/U8glib.h>

/** Define your pin number **/
#define BLUE 2      // Cold
#define RED 3       // Hot
#define YELLOW 4    // Moderate

U8GLIB_SSD1306_128X64 u8g(U8G_I2C_OPT_NONE | U8G_I2C_OPT_DEV_0);

int read_serial();

void draw_launch();

void draw_weather();

void lightLED();

void setup(void) {

    Serial.begin(9600);
    // flip screen, if required
    // u8g.setRot180();

    // set SPI backup if required
    //u8g.setHardwareBackup(u8g_backup_avr_spi);

    // assign default color value
    //if (u8g.getMode() == U8G_MODE_R3G3B2) {
    //    u8g.setColorIndex(255);     // white
    //} else if (u8g.getMode() == U8G_MODE_GRAY2BIT) {
    //    u8g.setColorIndex(3);         // max intensity
    //} else if (u8g.getMode() == U8G_MODE_BW) {
    //    u8g.setColorIndex(1);         // pixel on
    //} else if (u8g.getMode() == U8G_MODE_HICOLOR) {
    //    u8g.setHiColorByRGB(255, 255, 255);
    //}

    draw_launch();
}

String temp = "";
String temp_min = "";
String temp_max = "";
String pressure = "";
String humidity = "";
String time = "";

int current = 0;

void loop(void) {
    if (!read_serial()) {
        return;
    }

    if (current == 0) {
        digitalWrite(BLUE, LOW);
        digitalWrite(RED, LOW);
        digitalWrite(YELLOW, LOW);
    }

    draw_weather();
    lightLED();
    delay(100);
}

void draw_launch() {
    u8g.firstPage();
    do {
        u8g.setFont(u8g_font_unifont);
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth("1552980358")) / 2,
                    (u8g.getHeight() + u8g.getFontAscent()) / 2, "1552980358");
    } while (u8g.nextPage());

    pinMode(BLUE, OUTPUT);
    pinMode(RED, OUTPUT);
    pinMode(YELLOW, OUTPUT);

    digitalWrite(BLUE, HIGH);
    digitalWrite(RED, HIGH);
    digitalWrite(YELLOW, HIGH);

    delay(2000);
}

int read_serial() {

    if (!Serial.available()) {
        return 0;
    }

    auto msg = Serial.readString();

    Serial.write("RECEIVED\n");

    /**
     * Format:
     * 26;26;26;1004;75;2020-11-18 11:29
     *
     * Divided by ';'
     * Date and time will be fetched with hard-coded 16 letters
     **/

    temp = msg.substring(0, msg.indexOf(';'));
    // msg.replace(temp + ';', "");
    msg = msg.substring(msg.indexOf(';') + 1);
    // Serial.println(msg);

    temp_min = msg.substring(0, msg.indexOf(';'));
    // msg.replace(temp_min + ';', "");
    msg = msg.substring(msg.indexOf(';') + 1);
    // Serial.println(msg);

    temp_max = msg.substring(0, msg.indexOf(';'));
    // msg.replace(temp_max + ';', "");
    msg = msg.substring(msg.indexOf(';') + 1);
    // Serial.println(msg);

    pressure = msg.substring(0, msg.indexOf(';'));
    // msg.replace(pressure + ';', "");
    msg = msg.substring(msg.indexOf(';') + 1);
    // Serial.println(msg);

    humidity = msg.substring(0, msg.indexOf(';'));
    // msg.replace(humidity + ';', "");
    msg = msg.substring(msg.indexOf(';') + 1);
    // Serial.println(msg);

    // Date is fixed, in yyyy-MM-dd HH:mm
    // which is 16 letters
    time = msg.substring(0, 16);
    return 1;
}

void draw_weather() {
    u8g.firstPage();
    String tmp;
    int height = 0;
    do {
        u8g.setFont(u8g_font_10x20);
        height += u8g.getFontAscent();

        tmp = temp + "\'C";
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(tmp.c_str())) / 2, height, tmp.c_str());

        u8g.setFont(u8g_font_7x13);
        height += u8g.getFontAscent() * (1 + (1 / 2));

        tmp = temp_min + "\'C / " + temp_max + "\'C";
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(tmp.c_str())) / 2, height, tmp.c_str());

        height += u8g.getFontAscent() * 1.5;

        tmp = pressure + "hPa | " + humidity + '%';
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(tmp.c_str())) / 2, height, tmp.c_str());

        height += u8g.getFontAscent() * 1.5;
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(time.c_str())) / 2, height, time.c_str());

        height = 0;
    } while (u8g.nextPage());
}

void lightLED() {

    if (temp[0] <= '1') {
        if (current == BLUE) {
            return;
        }
        digitalWrite(current, LOW);
        current = BLUE;
        digitalWrite(current, HIGH);
        return;
    }

    if (temp[0] >= '3') {
        if (current == RED) {
            return;
        }
        digitalWrite(current, LOW);
        current = RED;
        digitalWrite(current, HIGH);
    }

    if (temp[0] == '2') {
        if (temp[1] <= '3') {
            if (current == BLUE) {
                return;
            }
            digitalWrite(current, LOW);
            current = BLUE;
            digitalWrite(current, HIGH);
            return;
        }
        if (temp[1] >= '7') {
            if (current == RED) {
                return;
            }
            digitalWrite(current, LOW);
            current = RED;
            digitalWrite(current, HIGH);
            return;
        }
        if (current == YELLOW) {
            return;
        }
        digitalWrite(current, HIGH);
        current = YELLOW;
        digitalWrite(current, HIGH);
    }

}