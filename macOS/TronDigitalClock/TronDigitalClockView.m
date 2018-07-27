//
//  TronDigitalClockView.m
//  TronDigitalClock
//
//  Created by Kenneth Ortego on 7/4/18.
//  Copyright © 2018 Kenneth Ortego. All rights reserved.
//

#import "TronDigitalClockView.h"

#define NSColorFromRGB(rgbValue) [NSColor colorWithRed:((float)((rgbValue & 0xFF0000) >> 16))/255.0 green:((float)((rgbValue & 0xFF00) >> 8))/255.0 blue:((float)(rgbValue & 0xFF))/255.0 alpha:1.0]

#define PIXEL_SIZE 2
#define FONT_ROWS 7
#define FONT_COLS 5
#define DIGIT_PADDING 1
#define DIGIT_SPACING 1


@implementation TronDigitalClockView

NSSize displaysize;
CGPoint startingPt;

int millisecondCounter;
int millisecondsD1Counter;
int millisecondsD2Counter;

int millisecondsD1;
int millisecondsD2;

int seconds;
int minutes;
int hours;
int days;

bool hasSeconds = NO;
bool hasMinutes = NO;
bool hasHours = NO;
bool hasDays = NO;


- (instancetype)initWithFrame:(NSRect)frame isPreview:(BOOL)isPreview
{
    self = [super initWithFrame:frame isPreview:isPreview];
    if (self) {
        [self setAnimationTimeInterval:1/1000.0];
    }
    return self;
}

- (void)startAnimation
{
    [super startAnimation];
}

- (void)stopAnimation
{
    [super stopAnimation];
}

- (void)drawRect:(NSRect)rect
{
    [super drawRect:rect];
}

- (void)animateOneFrame
{
    [self updateDigitUI];
    [self calculateDigits];
}

- (BOOL)hasConfigureSheet
{
    return NO;
}

- (NSWindow*)configureSheet
{
    return nil;
}

- (void)updateDigitUI {
    
    // set milliseconds
    NSString *txtStr = [NSString stringWithFormat: @"%d%d", millisecondsD2, millisecondsD1];

    if (hasSeconds) {
        // set seconds
        txtStr = [NSString stringWithFormat: @"%02d:%@", seconds, txtStr];

        if (hasMinutes) {
            // set minutes
            txtStr = [NSString stringWithFormat: @"%02d:%@", minutes, txtStr];

            if (hasHours) {
                // set hours
                txtStr = [NSString stringWithFormat: @"%02d:%@", hours, txtStr];

                if (hasDays) {
                    // set Days
                    txtStr = [NSString stringWithFormat: @"%02d:%@", days, txtStr];
                }
            }
        }
    }
    
    // set start position for this frame, center x,y
    startingPt = NSMakePoint(((self.bounds.size.width - (txtStr.length * (((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING))) / 2), ((self.bounds.size.height + (FONT_ROWS * (PIXEL_SIZE + DIGIT_SPACING))) / 2));
    
    
    [self drawDigits:txtStr];
    
}

- (void)calculateDigits {
    
    // D1 and D2 are for each of the milliseconds places
    millisecondsD1Counter++;
    millisecondsD2Counter++;
    millisecondCounter++;
    
    
    // increment D1 by 1 every frame
    millisecondsD1++;
    
    if (millisecondsD1 == 10) {
        millisecondsD1 = 0;
    }
    
    // increment D2 by 1 every 5 frames, to simulate fast ticking of milliseconds
    if (millisecondsD2Counter == 5) {
        millisecondsD2Counter = 0;
        millisecondsD2++;
        
        if (millisecondsD2 == 10) {
            millisecondsD2 = 0;
        }
    }
    
    // increment seconds every 60 frames
    if (millisecondCounter == 60 ) {
        millisecondCounter = 0;
        
        seconds++;
        hasSeconds = YES;
        
        
        // now that the seconds are good, the rest of the calculations line up
        if (seconds >= 60) {
            seconds = 0;
            
            minutes++;
            hasMinutes = YES;
            
            if (minutes >= 60) {
                minutes = 0;
                
                hours++;
                hasHours = YES;
                
                if (hours >= 60) {
                    hours = 0;
                    
                    days++;
                    hasDays = YES;
                }
            }
        }
    }
}
- (void)drawDigits: (NSString*)txt {
    
    NSGraphicsContext *context = [NSGraphicsContext currentContext];
    CGContextClearRect(context.CGContext, self.bounds);
    
    for (int i = 0; i < txt.length; i++) {
        
        NSString *c = [NSString stringWithFormat:@"%C", [txt characterAtIndex:i]];
        
        if ([c isEqualToString:@"0"]) {
            [self draw0];
        } else if ([c isEqualToString:@"1"]) {
            [self draw1];
        } else if ([c isEqualToString:@"2"]) {
            [self draw2];
        } else if ([c isEqualToString:@"3"]) {
            [self draw3];
        } else if ([c isEqualToString:@"4"]) {
            [self draw4];
        } else if ([c isEqualToString:@"5"]) {
            [self draw5];
        } else if ([c isEqualToString:@"6"]) {
            [self draw6];
        } else if ([c isEqualToString:@"7"]) {
            [self draw7];
        } else if ([c isEqualToString:@"8"]) {
            [self draw8];
        } else if ([c isEqualToString:@"9"]) {
            [self draw9];
        } else if ([c isEqualToString:@":"]) {
            [self drawColon];
        }
    }
    
}



- (void)drawColon{
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,0,0,0,0},
                                            {0,1,1,0,0},
                                            {0,1,1,0,0},
                                            {0,0,0,0,0},
                                            {0,1,1,0,0},
                                            {0,1,1,0,0},
                                            {0,0,0,0,0},};
    
    [self drawNumber:mappings];
}

- (void)draw0 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,1,1,1,0},
                                            {1,0,0,0,1},
                                            {1,0,0,1,1},
                                            {1,0,1,0,1},
                                            {1,1,0,0,1},
                                            {1,0,0,0,1},
                                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw1 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,0,1,0,0},
                            {0,1,1,0,0},
                            {0,0,1,0,0},
                            {0,0,1,0,0},
                            {0,0,1,0,0},
                            {0,0,1,0,0},
                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw2 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,1,1,1,0},
                            {1,0,0,0,1},
                            {0,0,0,0,1},
                            {0,0,0,1,0},
                            {0,0,1,0,0},
                            {0,1,0,0,0},
                            {1,1,1,1,1}};
    
    [self drawNumber:mappings];
}

- (void)draw3 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {1,1,1,1,1},
                            {0,0,0,1,0},
                            {0,0,1,0,0},
                            {0,0,0,1,0},
                            {0,0,0,0,1},
                            {1,0,0,0,1},
                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw4 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,0,0,1,0},
                            {0,0,1,1,0},
                            {0,1,0,1,0},
                            {1,0,0,1,0},
                            {1,1,1,1,1},
                            {0,0,0,1,0},
                            {0,0,0,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw5 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {1,1,1,1,1},
                                            {1,0,0,0,0,},
                                            {1,1,1,1,0},
                                            {0,0,0,0,1},
                                            {0,0,0,0,1},
                                            {1,0,0,0,1},
                                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw6 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,0,1,1,0},
                            {0,1,0,0,0},
                            {1,0,0,0,0},
                            {1,1,1,1,0},
                            {1,0,0,0,1},
                            {1,0,0,0,1},
                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw7 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {1,1,1,1,1},
                                            {0,0,0,0,1},
                                            {0,0,0,1,0},
                                            {0,0,1,0,0},
                                            {0,1,0,0,0},
                                            {0,1,0,0,0},
                                            {0,1,0,0,0}};
    
    [self drawNumber:mappings];
}

- (void)draw8 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,1,1,1,0},
                            {1,0,0,0,1},
                            {1,0,0,0,1},
                            {0,1,1,1,0},
                            {1,0,0,0,1},
                            {1,0,0,0,1},
                            {0,1,1,1,0}};
    
    [self drawNumber:mappings];
}

- (void)draw9 {
    
    int mappings [FONT_ROWS][FONT_COLS] = { {0,1,1,1,0},
                                            {1,0,0,0,1},
                                            {1,0,0,0,1},
                                            {0,1,1,1,1},
                                            {0,0,0,0,1},
                                            {0,0,0,1,0},
                                            {0,1,1,0,0}};
    
    [self drawNumber:mappings];
}

- (void)drawNumber:(int[FONT_ROWS][FONT_COLS])mappings {
    
    NSBezierPath * path;
    NSRect rect;
    
    rect.origin = startingPt;
    rect.size.width  = PIXEL_SIZE;
    rect.size.height = PIXEL_SIZE;
    
    [NSColorFromRGB(0x9DF6D8) set];
    
    for (int i = 0; i < FONT_ROWS; i++) {
        
        for (int j = 0; j < FONT_COLS; j++) {
            
            if (mappings[i][j] == 1) {
                path = [NSBezierPath bezierPathWithRect:rect];
                [path fill];
            }
            
            // move right one pixel
            rect.origin.x += (PIXEL_SIZE + DIGIT_SPACING);
        }
        
        // go back to beginning of line
        rect.origin.x = startingPt.x;
        
        // move down one pixel
        rect.origin.y -= (PIXEL_SIZE + DIGIT_SPACING);
    }
    
    // set up x position (magic number) for the next digit
    startingPt.x += ((FONT_COLS + 1) * (PIXEL_SIZE + DIGIT_SPACING)) + DIGIT_PADDING ;
    
}

- (void)writeLog:(NSString*)log {
    
    if (self.isPreview) {
        return;
    }
    
    NSString *filepath = @"/Users/kenneth/Documents/saver.txt";
    
    // create the file if its not there
    if (![[NSFileManager defaultManager] fileExistsAtPath:filepath]) {
        [[NSFileManager defaultManager] createFileAtPath:filepath contents:nil attributes:nil];
    }
    
    // create a timestamp for each log
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:@"yyyy-MM-dd hh:mm:ss"];
    NSString *timestamp = [dateFormatter stringFromDate:[NSDate date]];
    
    // grab the logs and append the new one
    NSString *logs = [NSString stringWithContentsOfFile:filepath encoding:NSUTF8StringEncoding error:nil];
    logs = [logs stringByAppendingString:[NSString stringWithFormat:@"%@: %@\n", timestamp, log]];
    
    // finally append to log to file
    [logs writeToFile:filepath atomically:YES encoding:NSUTF8StringEncoding error:nil];
    
}


@end
