//
//  Utils.m
//  AnythingFactory
//
//  Created by Meteora Van on 13-11-21.
//  Copyright (c) 2013年 mogo. All rights reserved.
//

#import "Utils.h"

extern "C"
{
    int  CreateMD5(char* pSrc,char *pMD5)
    {
        memset(pMD5, 0, 33);
        [Utils CreateMD5:pSrc dest:(char*)pMD5];
        return 0;
    }
}

@implementation Utils
+(int)CreateMD5:(char *)pSrc dest:(char *)pDest
{
    unsigned char result[32];
    CC_MD5(pSrc, strlen(pSrc), result);
    NSString *md5=[NSString stringWithFormat:@"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x", result[0], result[1], result[2], result[3], result[4], result[5], result[6], result[7], result[8], result[9], result[10], result[11], result[12], result[13], result[14], result[15]];
    memcpy(pDest, [md5 UTF8String], 32);
    return 0;
}

+(void)CreateNotification
{
    UILocalNotification *notification=[[UILocalNotification alloc] init];
    if (notification!=nil) {
        NSLog(@"Support local notification");
        NSDate *now=[NSDate new];
        notification.fireDate=[now addTimeInterval :10.0];
        notification.timeZone=[NSTimeZone defaultTimeZone];
        notification.alertBody=@"哎呦";
        [[UIApplication sharedApplication] scheduleLocalNotification:notification];
    }
}

@end
