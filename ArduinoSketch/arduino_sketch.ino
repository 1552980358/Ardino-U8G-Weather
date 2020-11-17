#include <Arduino.h>
#include <U8glib/U8glib.h>

U8GLIB_SSD1306_128X64 u8g(U8G_I2C_OPT_NONE | U8G_I2C_OPT_DEV_0);

void read_serial();

void draw_launch();

void draw_weather();

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

void loop(void) {
    read_serial();
    if (temp == "" || temp_min == "" || temp_max == "" || pressure == "" || humidity == "" || time == "") {
        draw_launch();
        return;
    }

    draw_weather();
}

void draw_launch() {
    u8g.firstPage();
    do {
        u8g.setFont(u8g_font_unifont);
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth("1552980358")) / 2,
                    (u8g.getHeight() + u8g.getFontAscent()) / 2, "1552980358");
    } while (u8g.nextPage());

    delay(2000);
}

void read_serial() {
    if (!Serial.available()) {
        return;
    }

    auto msg = Serial.readString();
    temp = msg.substring(0, msg.indexOf(';'));
    msg.replace(temp + ';', "");

    temp_min = msg.substring(0, msg.indexOf(';'));
    msg.replace(temp_min + ';', "");

    temp_max = msg.substring(0, msg.indexOf(';'));
    msg.replace(temp_max + ';', "");

    pressure = msg.substring(0, msg.indexOf(';'));
    msg.replace(pressure + ';', "");

    humidity = msg.substring(0, msg.indexOf(';'));
    msg.replace(humidity + ';', "");

    time = msg.substring(0, 16);
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
        height += u8g.getFontAscent() * 1.5;

        tmp = temp_min + "\'C / " + temp_max + "\'C";
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(tmp.c_str())) / 2, height, tmp.c_str());

        height += u8g.getFontAscent() * 1.5;

        tmp = pressure + "hPa | " + humidity + '%';
        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(tmp.c_str())) / 2, height, tmp.c_str());

        height += u8g.getFontAscent() * 1.5;

        u8g.drawStr((u8g.getWidth() - u8g.getStrPixelWidth(time.c_str())) / 2, height, time.c_str());

        height = 0;
    } while (u8g.nextPage());
    delay(1000);
}