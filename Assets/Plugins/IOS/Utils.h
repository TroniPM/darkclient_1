//
//  Utils.h
//  AnythingFactory
//
//  Created by Meteora Van on 13-11-21.
//  Copyright (c) 2013年 mogo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <commoncrypto/CommonDigest.h>

@interface Utils : NSObject

+(int)CreateMD5:(char *)pSrc dest:(char *)pDest;
+(void)CreateNotification;

@end
