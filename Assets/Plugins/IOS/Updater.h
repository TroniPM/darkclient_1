//
//  Updater.h
//  AnythingFactory
//
//  Created by Meteora Van on 13-11-14.
//  Copyright (c) 2013年 mogo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>



@interface Updater : NSObject


+(BOOL)ModuleDetectAll;
+(BOOL)ModuleDetect:(NSString*)functionName;
+(int)UpdateLoader:(NSString*)ipaPath;

-(int)Test;

@end
