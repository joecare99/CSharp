  private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_451b
        int try0001_dispatch = -1;
        int num = default;
        short index = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num6 = default;
        int num7 = default;
        int num8 = default;
        int num9 = default;
        string sDest = default;
        int num10 = default;
        string sDest2 = default;
        short num12 = default;
        int num14 = default;
        short num15 = default;
        int num17 = default;
        int num19 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    int i3;
                    int i4;
                    int i5;
                    int i6;
                    int i7;
                    short t;
                    short num13;
                    int i8;
                    int i9;
                    int i10;
                    short num16;
                    ListBox L;
                    short A;
                    int i11;
                    int i12;
                    int i13;
                    short num18;
                    int i14;
                    int i15;
                    int i16;
                    int i17;
                    int num5;
                    short t2;
                    short num20;
                    short num11;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command1.GetIndex((Button)eventSender);
                            goto IL_0016;
                        case 21059:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_45b9;
                                    default:
                                        goto end_IL_0001;
                                }
                                goto IL_451d;
                            }
                        IL_0c00:
                            num = 149;
                            if (!DataModul.TTable.NoMatch)
                            {
                                goto IL_0c1a;
                            }
                            else
                            {
                                goto IL_0c34;
                            }
                        IL_0c1a:
                            num = 150;
                            StringType.MidStmtStr(ref Modul1.LiText, 79, 1, "J");
                            goto IL_0c34;
                        IL_0ba2:
                            num = 148;
                            DataModul.TTable.Seek("=", 1, Modul1.PersInArb);
                            goto IL_0c00;
                        IL_451d:
                            num = 789;
                            if (Information.Err().Number == 6)
                            {
                                goto IL_4537;
                            }
                            else
                            {
                                goto IL_4563;
                            }
                        IL_4563:
                            num = 794;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_4593;
                        IL_0c6f:
                            num = 153;
                            Modul1.LiText = "";
                            goto IL_0c81;
                        IL_4593:
                            num = 797;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_45bd;
                        IL_0c81:
                            num = 154;
                            lErl = 5;
                            goto IL_0c8b;
                        IL_0c8b:
                            num = 155;
                            I1++;
                            goto IL_0ca1;
                        IL_45bd:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0016;
                                case 3:
                                    goto IL_0026;
                                case 4:
                                    goto IL_0036;
                                case 5:
                                    goto IL_003e;
                                case 7:
                                case 9:
                                    goto IL_006f;
                                case 10:
                                    goto IL_0080;
                                case 12:
                                case 13:
                                    goto IL_0094;
                                case 6:
                                case 11:
                                case 14:
                                case 15:
                                    goto IL_00a6;
                                case 16:
                                    goto IL_00bb;
                                case 17:
                                    goto IL_00db;
                                case 18:
                                case 19:
                                    goto IL_00ed;
                                case 20:
                                    goto IL_010d;
                                case 21:
                                case 22:
                                    goto IL_011f;
                                case 23:
                                    goto IL_0130;
                                case 24:
                                    goto IL_0141;
                                case 25:
                                    goto IL_0156;
                                case 26:
                                    goto IL_016b;
                                case 27:
                                case 28:
                                    goto IL_017c;
                                case 29:
                                    goto IL_0191;
                                case 30:
                                case 31:
                                    goto IL_01a2;
                                case 33:
                                case 35:
                                    goto IL_01d7;
                                case 36:
                                    goto IL_01ec;
                                case 37:
                                    goto IL_0207;
                                case 38:
                                    goto IL_0228;
                                case 39:
                                    goto IL_0249;
                                case 40:
                                    goto IL_027a;
                                case 41:
                                    goto IL_02ab;
                                case 42:
                                    goto IL_02b6;
                                case 43:
                                    goto IL_02d2;
                                case 44:
                                    goto IL_02f2;
                                case 45:
                                    goto IL_02f9;
                                case 47:
                                case 48:
                                    goto IL_0311;
                                case 49:
                                    goto IL_0325;
                                case 50:
                                    goto IL_0335;
                                case 51:
                                    goto IL_0359;
                                case 53:
                                case 54:
                                    goto IL_036e;
                                case 55:
                                    goto IL_039e;
                                case 57:
                                case 58:
                                    goto IL_03dd;
                                case 59:
                                    goto IL_03ee;
                                case 60:
                                    goto IL_03ff;
                                case 61:
                                    goto IL_0410;
                                case 62:
                                    goto IL_042c;
                                case 63:
                                    goto IL_0441;
                                case 64:
                                    goto IL_0456;
                                case 65:
                                    goto IL_0466;
                                case 66:
                                    goto IL_0470;
                                case 67:
                                    goto IL_047f;
                                case 69:
                                case 70:
                                    goto IL_0497;
                                case 71:
                                    goto IL_04f6;
                                case 73:
                                case 74:
                                    goto IL_050f;
                                case 75:
                                    goto IL_0519;
                                case 76:
                                    goto IL_0550;
                                case 77:
                                    goto IL_055b;
                                case 78:
                                    goto IL_0566;
                                case 79:
                                    goto IL_057f;
                                case 80:
                                    goto IL_0598;
                                case 81:
                                    goto IL_05ba;
                                case 82:
                                    goto IL_05c4;
                                case 83:
                                    goto IL_05cf;
                                case 84:
                                    goto IL_05f2;
                                case 85:
                                    goto IL_0615;
                                case 86:
                                    goto IL_0639;
                                case 87:
                                    goto IL_065d;
                                case 88:
                                    goto IL_0667;
                                case 89:
                                    goto IL_067b;
                                case 90:
                                    goto IL_06db;
                                case 92:
                                case 93:
                                    goto IL_06f4;
                                case 95:
                                case 97:
                                    goto IL_071b;
                                case 98:
                                    goto IL_074a;
                                case 99:
                                case 100:
                                    goto IL_0764;
                                case 101:
                                    goto IL_0793;
                                case 102:
                                case 103:
                                    goto IL_07ad;
                                case 104:
                                    goto IL_07dc;
                                case 105:
                                case 106:
                                    goto IL_07f6;
                                case 107:
                                    goto IL_082e;
                                case 110:
                                case 111:
                                    goto IL_084e;
                                case 112:
                                    goto IL_087d;
                                case 113:
                                case 114:
                                    goto IL_0897;
                                case 115:
                                    goto IL_08c6;
                                case 116:
                                case 117:
                                    goto IL_08e0;
                                case 118:
                                    goto IL_090f;
                                case 119:
                                case 120:
                                    goto IL_0929;
                                case 121:
                                    goto IL_0961;
                                case 124:
                                case 125:
                                    goto IL_0981;
                                case 126:
                                    goto IL_09b0;
                                case 127:
                                case 128:
                                    goto IL_09ca;
                                case 129:
                                    goto IL_09fc;
                                case 130:
                                case 131:
                                    goto IL_0a19;
                                case 132:
                                    goto IL_0a4b;
                                case 133:
                                case 134:
                                    goto IL_0a68;
                                case 135:
                                    goto IL_0aa3;
                                case 91:
                                case 94:
                                case 108:
                                case 109:
                                case 122:
                                case 123:
                                case 136:
                                case 137:
                                case 138:
                                    goto IL_0ac2;
                                case 139:
                                    goto IL_0acc;
                                case 140:
                                    goto IL_0ae8;
                                case 141:
                                    goto IL_0b00;
                                case 142:
                                    goto IL_0b0d;
                                case 143:
                                    goto IL_0b40;
                                case 144:
                                    goto IL_0b71;
                                case 145:
                                case 147:
                                    goto IL_0b8b;
                                case 148:
                                    goto IL_0ba2;
                                case 149:
                                    goto IL_0c00;
                                case 150:
                                    goto IL_0c1a;
                                case 151:
                                case 152:
                                    goto IL_0c34;
                                case 153:
                                    goto IL_0c6f;
                                case 72:
                                case 154:
                                    goto IL_0c81;
                                case 155:
                                    goto IL_0c8b;
                                case 156:
                                    goto IL_0cb2;
                                case 158:
                                case 159:
                                    goto IL_0cef;
                                case 160:
                                    goto IL_0d06;
                                case 161:
                                    goto IL_0d1e;
                                case 162:
                                    goto IL_0d3c;
                                case 163:
                                    goto IL_0d5a;
                                case 164:
                                    goto IL_0d78;
                                case 165:
                                    goto IL_0d90;
                                case 166:
                                case 167:
                                    goto IL_0dad;
                                case 168:
                                    goto IL_0dc5;
                                case 169:
                                case 170:
                                    goto IL_0de2;
                                case 171:
                                    goto IL_0dfa;
                                case 172:
                                case 173:
                                    goto IL_0e17;
                                case 174:
                                    goto IL_0e24;
                                case 175:
                                    goto IL_0e42;
                                case 176:
                                    goto IL_0e62;
                                case 177:
                                    goto IL_0e6c;
                                case 179:
                                case 180:
                                    goto IL_0e87;
                                case 181:
                                    goto IL_0e9a;
                                case 182:
                                    goto IL_0ec1;
                                case 183:
                                    goto IL_0ef4;
                                case 185:
                                case 186:
                                    goto IL_0f36;
                                case 187:
                                    goto IL_0f4a;
                                case 188:
                                    goto IL_0f69;
                                case 189:
                                    goto IL_0f7d;
                                case 190:
                                    goto IL_0f95;
                                case 191:
                                    goto IL_0fa8;
                                case 192:
                                    goto IL_0fb5;
                                case 193:
                                    goto IL_0fc3;
                                case 194:
                                    goto IL_0fd5;
                                case 196:
                                case 197:
                                    goto IL_0ff0;
                                case 198:
                                    goto IL_1007;
                                case 199:
                                    goto IL_1069;
                                case 201:
                                case 202:
                                    goto IL_1085;
                                case 203:
                                    goto IL_1092;
                                case 204:
                                    goto IL_10aa;
                                case 205:
                                    goto IL_10bb;
                                case 206:
                                    goto IL_10c8;
                                case 207:
                                    goto IL_10ed;
                                case 209:
                                    goto IL_1108;
                                case 210:
                                    goto IL_1110;
                                case 208:
                                case 211:
                                case 212:
                                    goto IL_1128;
                                case 213:
                                case 214:
                                    goto IL_1140;
                                case 215:
                                    goto IL_1158;
                                case 216:
                                    goto IL_1169;
                                case 217:
                                    goto IL_1176;
                                case 218:
                                    goto IL_119b;
                                case 220:
                                    goto IL_11b7;
                                case 221:
                                    goto IL_11bf;
                                case 219:
                                case 222:
                                case 223:
                                case 224:
                                    goto IL_11d9;
                                case 225:
                                    goto IL_11f3;
                                case 226:
                                case 227:
                                    goto IL_1210;
                                case 228:
                                    goto IL_1227;
                                case 229:
                                    goto IL_123e;
                                case 230:
                                    goto IL_129b;
                                case 232:
                                case 233:
                                    goto IL_12b7;
                                case 234:
                                    goto IL_12e9;
                                case 235:
                                case 236:
                                    goto IL_1306;
                                case 237:
                                    goto IL_1313;
                                case 238:
                                    goto IL_1320;
                                case 239:
                                    goto IL_138c;
                                case 241:
                                case 242:
                                    goto IL_13a8;
                                case 244:
                                case 246:
                                    goto IL_13e2;
                                case 247:
                                    goto IL_1414;
                                case 248:
                                case 249:
                                    goto IL_1431;
                                case 250:
                                    goto IL_146c;
                                case 253:
                                case 254:
                                    goto IL_148f;
                                case 255:
                                    goto IL_14c1;
                                case 256:
                                case 257:
                                    goto IL_14de;
                                case 258:
                                    goto IL_1519;
                                case 261:
                                case 262:
                                    goto IL_153c;
                                case 263:
                                    goto IL_156e;
                                case 264:
                                case 265:
                                    goto IL_158b;
                                case 266:
                                    goto IL_15c6;
                                case 269:
                                case 270:
                                    goto IL_15e9;
                                case 271:
                                    goto IL_161b;
                                case 272:
                                case 273:
                                    goto IL_1638;
                                case 274:
                                    goto IL_1673;
                                case 277:
                                case 278:
                                    goto IL_1696;
                                case 279:
                                    goto IL_16c8;
                                case 280:
                                case 281:
                                    goto IL_16e5;
                                case 282:
                                    goto IL_1720;
                                case 285:
                                case 286:
                                    goto IL_1743;
                                case 287:
                                    goto IL_1775;
                                case 288:
                                case 289:
                                    goto IL_1792;
                                case 290:
                                    goto IL_17cd;
                                case 240:
                                case 243:
                                case 251:
                                case 252:
                                case 259:
                                case 260:
                                case 267:
                                case 268:
                                case 275:
                                case 276:
                                case 283:
                                case 284:
                                case 291:
                                case 292:
                                case 293:
                                    goto IL_17ec;
                                case 294:
                                    goto IL_17f7;
                                case 295:
                                    goto IL_1814;
                                case 296:
                                    goto IL_181e;
                                case 297:
                                    goto IL_188a;
                                case 298:
                                    goto IL_18a7;
                                case 299:
                                    goto IL_18d9;
                                case 300:
                                case 301:
                                    goto IL_18f0;
                                case 302:
                                    goto IL_192b;
                                case 303:
                                case 304:
                                case 305:
                                    goto IL_1943;
                                case 200:
                                case 231:
                                case 306:
                                    goto IL_197c;
                                case 307:
                                    goto IL_1987;
                                case 308:
                                    goto IL_19ae;
                                case 309:
                                    goto IL_19e7;
                                case 310:
                                    goto IL_19ff;
                                case 313:
                                case 314:
                                    goto IL_1a3c;
                                case 315:
                                    goto IL_1a54;
                                case 316:
                                    goto IL_1a6c;
                                case 317:
                                    goto IL_1a80;
                                case 318:
                                    goto IL_1a94;
                                case 321:
                                case 322:
                                    goto IL_1aae;
                                case 323:
                                    goto IL_1acb;
                                case 324:
                                    goto IL_1aed;
                                case 325:
                                    goto IL_1b1c;
                                case 326:
                                    goto IL_1b4b;
                                case 327:
                                    goto IL_1b7a;
                                case 328:
                                    goto IL_1b92;
                                case 330:
                                    goto IL_1bb9;
                                case 331:
                                    goto IL_1bc1;
                                case 329:
                                case 332:
                                case 333:
                                    goto IL_1be7;
                                case 335:
                                case 336:
                                    goto IL_1c00;
                                case 337:
                                    goto IL_1c0e;
                                case 338:
                                    goto IL_1c26;
                                case 339:
                                    goto IL_1c44;
                                case 340:
                                    goto IL_1c62;
                                case 341:
                                    goto IL_1c80;
                                case 342:
                                    goto IL_1c98;
                                case 343:
                                case 344:
                                    goto IL_1cb5;
                                case 345:
                                    goto IL_1ccd;
                                case 346:
                                case 347:
                                    goto IL_1cea;
                                case 348:
                                    goto IL_1d02;
                                case 349:
                                case 350:
                                    goto IL_1d1f;
                                case 351:
                                    goto IL_1d2c;
                                case 352:
                                    goto IL_1d4a;
                                case 353:
                                    goto IL_1d6a;
                                case 354:
                                    goto IL_1d74;
                                case 356:
                                case 357:
                                    goto IL_1d8f;
                                case 358:
                                    goto IL_1da2;
                                case 359:
                                    goto IL_1dc9;
                                case 360:
                                    goto IL_1dfc;
                                case 362:
                                case 363:
                                    goto IL_1e3d;
                                case 364:
                                    goto IL_1e51;
                                case 365:
                                    goto IL_1e65;
                                case 366:
                                    goto IL_1e79;
                                case 367:
                                    goto IL_1e9f;
                                case 368:
                                    goto IL_1ec2;
                                case 369:
                                    goto IL_1ed5;
                                case 370:
                                    goto IL_1ee2;
                                case 371:
                                    goto IL_1ef0;
                                case 372:
                                    goto IL_1f02;
                                case 374:
                                case 375:
                                    goto IL_1f1d;
                                case 376:
                                    goto IL_1f34;
                                case 377:
                                    goto IL_1f96;
                                case 379:
                                case 380:
                                    goto IL_1fb2;
                                case 381:
                                    goto IL_1fbf;
                                case 382:
                                    goto IL_1fd7;
                                case 383:
                                    goto IL_1fe8;
                                case 384:
                                    goto IL_1ff5;
                                case 385:
                                    goto IL_201a;
                                case 387:
                                    goto IL_2035;
                                case 388:
                                    goto IL_203d;
                                case 386:
                                case 389:
                                case 390:
                                    goto IL_2055;
                                case 391:
                                case 392:
                                    goto IL_206d;
                                case 393:
                                    goto IL_2085;
                                case 394:
                                    goto IL_2096;
                                case 395:
                                    goto IL_20a3;
                                case 396:
                                    goto IL_20c8;
                                case 398:
                                    goto IL_20e4;
                                case 399:
                                    goto IL_20ec;
                                case 397:
                                case 400:
                                case 401:
                                case 402:
                                    goto IL_2106;
                                case 403:
                                    goto IL_2120;
                                case 404:
                                case 405:
                                    goto IL_213d;
                                case 406:
                                    goto IL_2154;
                                case 407:
                                    goto IL_216b;
                                case 408:
                                    goto IL_21c8;
                                case 410:
                                case 411:
                                    goto IL_21e4;
                                case 412:
                                    goto IL_2216;
                                case 413:
                                case 414:
                                    goto IL_2233;
                                case 415:
                                    goto IL_2240;
                                case 416:
                                    goto IL_224d;
                                case 417:
                                    goto IL_22b9;
                                case 419:
                                case 420:
                                    goto IL_22d8;
                                case 422:
                                case 423:
                                    goto IL_22fa;
                                case 378:
                                case 409:
                                case 421:
                                case 424:
                                    goto IL_2317;
                                case 425:
                                    goto IL_2322;
                                case 426:
                                    goto IL_238e;
                                case 428:
                                case 429:
                                    goto IL_23aa;
                                case 430:
                                    goto IL_23cf;
                                case 431:
                                    goto IL_2407;
                                case 373:
                                case 418:
                                case 427:
                                case 432:
                                case 433:
                                    goto IL_241f;
                                case 434:
                                    goto IL_242a;
                                case 435:
                                    goto IL_2451;
                                case 438:
                                case 439:
                                    goto IL_2496;
                                case 440:
                                    goto IL_24ae;
                                case 441:
                                    goto IL_24cc;
                                case 442:
                                    goto IL_24f0;
                                case 443:
                                    goto IL_2514;
                                case 444:
                                    goto IL_2522;
                                case 445:
                                    goto IL_2541;
                                case 446:
                                    goto IL_2564;
                                case 447:
                                    goto IL_256e;
                                case 449:
                                case 450:
                                    goto IL_2589;
                                case 451:
                                    goto IL_25a0;
                                case 452:
                                    goto IL_25b3;
                                case 453:
                                    goto IL_25da;
                                case 455:
                                case 456:
                                    goto IL_25f2;
                                case 457:
                                    goto IL_2625;
                                case 459:
                                case 460:
                                    goto IL_2666;
                                case 461:
                                    goto IL_267a;
                                case 462:
                                    goto IL_268e;
                                case 463:
                                    goto IL_26a2;
                                case 464:
                                    goto IL_26c1;
                                case 465:
                                    goto IL_26e3;
                                case 466:
                                    goto IL_26f6;
                                case 467:
                                    goto IL_2703;
                                case 468:
                                    goto IL_2715;
                                case 470:
                                case 471:
                                    goto IL_2730;
                                case 472:
                                    goto IL_2792;
                                case 474:
                                case 475:
                                    goto IL_27ae;
                                case 476:
                                    goto IL_27bb;
                                case 477:
                                    goto IL_27f5;
                                case 478:
                                    goto IL_2803;
                                case 479:
                                    goto IL_2811;
                                case 480:
                                    goto IL_282d;
                                case 481:
                                    goto IL_2849;
                                case 482:
                                    goto IL_286e;
                                case 483:
                                    goto IL_287b;
                                case 484:
                                    goto IL_2889;
                                case 486:
                                case 487:
                                    goto IL_28b9;
                                case 489:
                                case 490:
                                    goto IL_28e9;
                                case 492:
                                case 493:
                                    goto IL_2919;
                                case 495:
                                case 496:
                                    goto IL_2949;
                                case 497:
                                    goto IL_2956;
                                case 498:
                                    goto IL_296d;
                                case 499:
                                    goto IL_29d0;
                                case 501:
                                case 502:
                                    goto IL_29ec;
                                case 504:
                                case 506:
                                    goto IL_2a16;
                                case 507:
                                    goto IL_2a48;
                                case 508:
                                case 509:
                                    goto IL_2a65;
                                case 511:
                                case 512:
                                    goto IL_2a9c;
                                case 514:
                                case 515:
                                    goto IL_2ad3;
                                case 516:
                                    goto IL_2b0e;
                                case 519:
                                case 520:
                                    goto IL_2b31;
                                case 521:
                                    goto IL_2b63;
                                case 522:
                                case 523:
                                    goto IL_2b80;
                                case 525:
                                case 526:
                                    goto IL_2bb7;
                                case 528:
                                case 529:
                                    goto IL_2bee;
                                case 530:
                                    goto IL_2c29;
                                case 533:
                                case 534:
                                    goto IL_2c4c;
                                case 535:
                                    goto IL_2c7e;
                                case 536:
                                case 537:
                                    goto IL_2c9b;
                                case 539:
                                case 540:
                                    goto IL_2cd2;
                                case 542:
                                case 543:
                                    goto IL_2d09;
                                case 544:
                                    goto IL_2d44;
                                case 500:
                                case 503:
                                case 517:
                                case 518:
                                case 531:
                                case 532:
                                case 545:
                                case 546:
                                case 547:
                                    goto IL_2d63;
                                case 548:
                                    goto IL_2d6e;
                                case 550:
                                    num = 550;
                                    Modul1.PersInArbsp = Modul1.PersInArb;
                                    goto case 551;
                                case 551:
                                    num = 551;
                                    Modul1.Kenn = 1f;
                                    goto case 552;
                                case 552:
                                    num = 552;
                                    if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.Sex)].Value, "F", TextCompare: false))
                                    {
                                        goto case 553;
                                    }
                                    goto case 554;
                                case 553:
                                    num = 553;
                                    Modul1.Kenn = 2f;
                                    goto case 554;
                                case 554:
                                case 555:
                                    num = 555;
                                    DataModul.DB_LinkTable.Index = nameof(DataModul.LinkIndex.ElSu);
                                    goto case 556;
                                case 556:
                                    num = 556;
                                    DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), Modul1.Kenn);
                                    goto case 557;
                                case 557:
                                    num = 557;
                                    if (!DataModul.DB_LinkTable.NoMatch)
                                    {
                                        goto case 559;
                                    }
                                    goto IL_3320;
                                case 559:
                                    num = 559;
                                    goto case 560;
                                case 560:
                                    num = 560;
                                    Modul1.FamInArb = DataModul.DB_LinkTable.Fields[nameof(DataModul.LinkFields.FamNr)].Value.AsInt();
                                    goto case 561;
                                case 561:
                                    num = 561;
                                    MainProject.Forms.Familie.Famles(Modul1.FamInArb);
                                    goto case 562;
                                case 562:
                                    num = 562;
                                    if (Modul1.Kenn == 1f)
                                    {
                                        goto case 563;
                                    }
                                    goto case 565;
                                case 563:
                                    num = 563;
                                    Modul1.PersInArb = Modul1.Family.Frau;
                                    goto case 564;
                                case 565:
                                    num = 565;
                                    if (Modul1.Kenn == 2f)
                                    {
                                        goto case 566;
                                    }
                                    goto case 564;
                                case 566:
                                    num = 566;
                                    Modul1.PersInArb = Modul1.Family.Mann;
                                    goto case 564;
                                case 564:
                                case 567:
                                case 568:
                                    num = 568;
                                    DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.ArtNr);
                                    goto case 569;
                                case 569:
                                    num = 569;
                                    Modul1.I = 101;
                                    goto case 570;
                                case 570:
                                    num = 570;
                                    Modul1.Ubg = Modul1.I;
                                    goto case 571;
                                case 571:
                                    num = 571;
                                    DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
                                    goto case 572;
                                case 572:
                                    num = 572;
                                    if (!DataModul.DB_EventTable.NoMatch)
                                    {
                                        goto case 574;
                                    }
                                    goto case 573;
                                case 574:
                                case 575:
                                    num = 575;
                                    if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                                    {
                                        goto case 576;
                                    }
                                    goto case 573;
                                case 576:
                                    num = 576;
                                    if (Conversions.ToDouble(Strings.Trim(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].AsString())) > 0.0)
                                    {
                                        goto case 577;
                                    }
                                    goto case 573;
                                case 577:
                                    num = 577;
                                    transdat = DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value.AsInt();
                                    goto case 578;
                                case 578:
                                    num = 578;
                                    neuedat();
                                    goto IL_3395;
                                case 573:
                                case 580:
                                case 581:
                                case 582:
                                    num = 582;
                                    lErl = 6;
                                    goto case 583;
                                case 583:
                                    {
                                        num = 583;
                                        Modul1.I++;
                                        int i = Modul1.I;
                                        num5 = 102;
                                        if (i <= num5)
                                        {
                                            goto case 570;
                                        }
                                        goto case 584;
                                    }
                                case 584:
                                    num = 584;
                                    num6 = 30000000;
                                    goto case 585;
                                case 585:
                                    num = 585;
                                    this.A = 1;
                                    goto case 586;
                                case 586:
                                    num = 586;
                                    if (Modul1.Family.Kind[this.A] > 0)
                                    {
                                        goto case 587;
                                    }
                                    goto case 598;
                                case 587:
                                    num = 587;
                                    Modul1.I = 101;
                                    goto case 588;
                                case 588:
                                    num = 588;
                                    DataModul.DB_EventTable.Seek("=", Modul1.I.AsString(), Modul1.Family.Kind[this.A].AsString(), "0");
                                    goto case 589;
                                case 589:
                                    num = 589;
                                    if (!DataModul.DB_EventTable.NoMatch)
                                    {
                                        goto case 590;
                                    }
                                    goto case 593;
                                case 590:
                                    num = 590;
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value) > 0.0)
                                    {
                                        goto case 591;
                                    }
                                    goto case 593;
                                case 591:
                                    num = 591;
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value) < num6)
                                    {
                                        goto case 592;
                                    }
                                    goto case 593;
                                case 592:
                                    num = 592;
                                    num6 = (int)Math.Round(Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value));
                                    goto case 593;
                                case 593:
                                case 594:
                                case 595:
                                case 596:
                                    {
                                        num = 596;
                                        Modul1.I++;
                                        int i2 = Modul1.I;
                                        num5 = 102;
                                        if (i2 <= num5)
                                        {
                                            goto case 588;
                                        }
                                        goto case 597;
                                    }
                                case 598:
                                    num = 598;
                                    goto case 599;
                                case 597:
                                case 600:
                                case 601:
                                    {
                                        num = 601;
                                        this.A++;
                                        int a = this.A;
                                        num5 = 99;
                                        if (a <= num5)
                                        {
                                            goto case 586;
                                        }
                                        goto case 599;
                                    }
                                case 599:
                                case 602:
                                    num = 602;
                                    if (num6 < 30000000)
                                    {
                                        goto case 603;
                                    }
                                    goto IL_32fc;
                                case 603:
                                    num = 603;
                                    transdat = Conversions.ToInteger(Conversion.Str(Conversions.ToDouble((num6).AsString().Left( 4)) - 25.0) + "0000");
                                    goto case 604;
                                case 604:
                                    num = 604;
                                    neuedat();
                                    goto IL_32fc;
                                case 549:
                                case 605:
                                case 606:
                                    goto IL_32fc;
                                case 607:
                                    goto IL_3307;
                                case 558:
                                case 608:
                                case 609:
                                    goto IL_3320;
                                case 610:
                                    goto IL_3348;
                                case 611:
                                case 612:
                                    goto IL_3383;
                                case 473:
                                case 485:
                                case 488:
                                case 491:
                                case 494:
                                case 510:
                                case 513:
                                case 524:
                                case 527:
                                case 538:
                                case 541:
                                case 579:
                                case 613:
                                    goto IL_3395;
                                case 614:
                                    goto IL_33a0;
                                case 615:
                                    goto IL_33c7;
                                case 617:
                                case 618:
                                    goto IL_3406;
                                case 619:
                                    goto IL_341e;
                                case 620:
                                    goto IL_343f;
                                case 621:
                                    goto IL_345d;
                                case 622:
                                    goto IL_3467;
                                case 624:
                                case 625:
                                    goto IL_3482;
                                case 626:
                                    goto IL_3499;
                                case 627:
                                    goto IL_34ac;
                                case 628:
                                    goto IL_34d3;
                                case 630:
                                case 631:
                                    goto IL_34eb;
                                case 632:
                                    goto IL_351e;
                                case 634:
                                case 635:
                                    goto IL_355f;
                                case 636:
                                    goto IL_357d;
                                case 637:
                                    goto IL_358a;
                                case 638:
                                    goto IL_359e;
                                case 639:
                                    goto IL_35b2;
                                case 640:
                                    goto IL_35c6;
                                case 641:
                                    goto IL_35e5;
                                case 642:
                                    goto IL_3607;
                                case 643:
                                    goto IL_361a;
                                case 644:
                                    goto IL_3627;
                                case 645:
                                    goto IL_3639;
                                case 646:
                                    goto IL_364b;
                                case 648:
                                case 649:
                                    goto IL_3666;
                                case 650:
                                    goto IL_36c8;
                                case 652:
                                case 653:
                                    goto IL_36e4;
                                case 654:
                                    goto IL_36f2;
                                case 655:
                                    goto IL_3700;
                                case 656:
                                    goto IL_370e;
                                case 657:
                                    goto IL_371f;
                                case 658:
                                    goto IL_3736;
                                case 659:
                                    goto IL_379d;
                                case 660:
                                    goto IL_37b4;
                                case 661:
                                case 662:
                                    goto IL_37c2;
                                case 663:
                                    goto IL_37d3;
                                case 664:
                                    goto IL_37e0;
                                case 665:
                                    goto IL_3811;
                                case 666:
                                case 667:
                                    goto IL_3822;
                                case 668:
                                    goto IL_3839;
                                case 669:
                                    goto IL_38a0;
                                case 670:
                                    goto IL_38b7;
                                case 671:
                                case 672:
                                    goto IL_38c5;
                                case 673:
                                    goto IL_38e8;
                                case 674:
                                    goto IL_38f5;
                                case 675:
                                    goto IL_3935;
                                case 676:
                                    goto IL_394b;
                                case 677:
                                case 678:
                                    goto IL_3962;
                                case 679:
                                    goto IL_3978;
                                case 680:
                                case 681:
                                    goto IL_398f;
                                case 682:
                                    goto IL_39f2;
                                case 683:
                                    goto IL_3a0c;
                                case 684:
                                case 685:
                                    goto IL_3a23;
                                case 686:
                                    goto IL_3a86;
                                case 687:
                                    goto IL_3aa0;
                                case 688:
                                case 689:
                                    goto IL_3ab7;
                                case 690:
                                    goto IL_3b1a;
                                case 691:
                                    goto IL_3b34;
                                case 692:
                                case 693:
                                    goto IL_3b4b;
                                case 694:
                                    goto IL_3bae;
                                case 695:
                                    goto IL_3bc8;
                                case 696:
                                case 697:
                                    goto IL_3bdf;
                                case 698:
                                    goto IL_3c42;
                                case 699:
                                    goto IL_3c5c;
                                case 700:
                                case 701:
                                    goto IL_3c73;
                                case 702:
                                    goto IL_3cd7;
                                case 703:
                                    goto IL_3cf1;
                                case 704:
                                case 705:
                                    goto IL_3d08;
                                case 706:
                                    goto IL_3d1f;
                                case 707:
                                    goto IL_3d7c;
                                case 708:
                                    goto IL_3d96;
                                case 709:
                                case 710:
                                    goto IL_3dad;
                                case 651:
                                case 711:
                                case 712:
                                    goto IL_3de8;
                                case 713:
                                    goto IL_3e0f;
                                case 715:
                                case 716:
                                    goto IL_3e4e;
                                case 717:
                                    goto IL_3e62;
                                case 718:
                                    goto IL_3e7a;
                                case 719:
                                    goto IL_3e84;
                                case 721:
                                case 722:
                                    goto IL_3e9f;
                                case 723:
                                    goto IL_3ebd;
                                case 724:
                                    goto IL_3edb;
                                case 725:
                                    goto IL_3ef9;
                                case 726:
                                    goto IL_3f16;
                                case 727:
                                    goto IL_3f33;
                                case 728:
                                    goto IL_3f45;
                                case 729:
                                    goto IL_3f5c;
                                case 730:
                                    goto IL_3fb6;
                                case 731:
                                    goto IL_3fc4;
                                case 734:
                                    goto IL_3fe6;
                                case 735:
                                    goto IL_3ffb;
                                case 736:
                                    goto IL_4044;
                                case 737:
                                    goto IL_4076;
                                case 738:
                                case 739:
                                    goto IL_4091;
                                case 740:
                                    goto IL_40c3;
                                case 741:
                                case 742:
                                    goto IL_40de;
                                case 743:
                                    goto IL_4110;
                                case 744:
                                case 745:
                                    goto IL_412b;
                                case 746:
                                    goto IL_415d;
                                case 747:
                                case 748:
                                    goto IL_4178;
                                case 749:
                                    goto IL_41aa;
                                case 750:
                                case 751:
                                    goto IL_41c5;
                                case 752:
                                    goto IL_4203;
                                case 753:
                                case 754:
                                    goto IL_421e;
                                case 755:
                                    goto IL_4259;
                                case 756:
                                case 757:
                                    goto IL_4274;
                                case 758:
                                    goto IL_42af;
                                case 759:
                                case 760:
                                    goto IL_42ca;
                                case 761:
                                    goto IL_4308;
                                case 762:
                                case 763:
                                    goto IL_4323;
                                case 764:
                                    goto IL_4361;
                                case 765:
                                case 766:
                                    goto IL_437c;
                                case 767:
                                    goto IL_43ba;
                                case 768:
                                case 769:
                                    goto IL_43d5;
                                case 770:
                                    goto IL_43f2;
                                case 771:
                                    goto IL_4407;
                                case 732:
                                case 733:
                                case 772:
                                    goto IL_441a;
                                case 774:
                                    goto IL_4439;
                                case 775:
                                    goto IL_4441;
                                case 773:
                                case 776:
                                case 777:
                                    goto IL_4454;
                                case 778:
                                    goto IL_446b;
                                case 32:
                                case 68:
                                case 157:
                                case 195:
                                case 311:
                                case 312:
                                case 320:
                                case 334:
                                case 437:
                                case 469:
                                case 616:
                                case 647:
                                case 714:
                                case 779:
                                case 780:
                                case 781:
                                    goto IL_448a;
                                case 782:
                                    goto IL_4494;
                                case 783:
                                    goto IL_44ac;
                                case 784:
                                    goto IL_44c9;
                                case 785:
                                    goto IL_44d6;
                                case 786:
                                    goto IL_44f3;
                                case 789:
                                    goto IL_451d;
                                case 790:
                                    goto IL_4537;
                                case 791:
                                    goto IL_4545;
                                case 792:
                                case 793:
                                case 794:
                                    goto IL_4563;
                                case 795:
                                case 797:
                                    goto IL_4593;
                                default:
                                    goto end_IL_0001;
                                case 46:
                                case 52:
                                case 56:
                                case 178:
                                case 184:
                                case 319:
                                case 355:
                                case 361:
                                case 436:
                                case 448:
                                case 454:
                                case 458:
                                case 623:
                                case 629:
                                case 633:
                                case 720:
                                case 787:
                                case 788:
                                case 798:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                        IL_4537:
                            num = 790;
                            num7 = 200000000;
                            goto IL_4545;
                        IL_4545:
                            num = 791;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_45b9;
                        IL_0c34:
                            num = 152;
                            List1.Items.Add(new ListItem(Modul1.LiText + "          " + Modul1.PersInArb.AsString()));
                            goto IL_0c6f;
                        IL_45b9:
                            num4 = unchecked(num2 + 1);
                            goto IL_45bd;
                        IL_0016:
                            num = 2;
                            RadioButton1.Visible = true;
                            goto IL_0026;
                        IL_0026:
                            num = 3;
                            RadioButton2.Visible = true;
                            goto IL_0036;
                        IL_0036:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_003e;
                        IL_003e:
                            num = 5;
                            switch (index)
                            {
                                case 0:
                                case 1:
                                case 4:
                                case 5:
                                case 6:
                                    break;
                                case 7:
                                    goto IL_0094;
                                default:
                                    goto IL_00a6;
                            }

                            {
                                goto IL_006f;
                            }
                        IL_0094:
                            num = 13;
                            List1.Visible = false;
                            goto IL_00a6;
                        IL_006f:
                            num = 9;
                            List2.Visible = false;
                            goto IL_0080;
                        IL_0080:
                            num = 10;
                            List1.Visible = true;
                            goto IL_00a6;
                        IL_00a6:
                            num = 15;
                            Label4.Text = "";
                            goto IL_00bb;
                        IL_00bb:
                            num = 16;
                            if (Modul1.Aus[12] == "")
                            {
                                goto IL_00db;
                            }
                            else
                            {
                                goto IL_00ed;
                            }
                        IL_00db:
                            num = 17;
                            Modul1.Aus[12] = "200";
                            goto IL_00ed;
                        IL_00ed:
                            num = 19;
                            if (Modul1.Aus[13] == "")
                            {
                                goto IL_010d;
                            }
                            else
                            {
                                goto IL_011f;
                            }
                        IL_010d:
                            num = 20;
                            Modul1.Aus[13] = "200";
                            goto IL_011f;
                        IL_011f:
                            num = 22;
                            List1.Visible = true;
                            goto IL_0130;
                        IL_0130:
                            num = 23;
                            List2.Visible = false;
                            goto IL_0141;
                        IL_0141:
                            num = 24;
                            Label2.Text = "";
                            goto IL_0156;
                        IL_0156:
                            num = 25;
                            if (RadioButton1.Checked)
                            {
                                goto IL_016b;
                            }
                            else
                            {
                                goto IL_017c;
                            }
                        IL_016b:
                            num = 26;
                            List1.Sorted = true;
                            goto IL_017c;
                        IL_017c:
                            num = 28;
                            if (RadioButton2.Checked)
                            {
                                goto IL_0191;
                            }
                            else
                            {
                                goto IL_01a2;
                            }
                        IL_0191:
                            num = 29;
                            List1.Sorted = false;
                            goto IL_01a2;
                        IL_01a2:
                            num = 31;
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_0cef;
                                case 2:
                                    goto IL_1a3c;
                                case 3:
                                    goto IL_1aae;
                                case 4:
                                    goto IL_1c00;
                                case 5:
                                    goto IL_2496;
                                case 6:
                                    goto IL_3406;
                                case 7:
                                    goto IL_3e4e;
                                default:
                                    goto IL_448a;
                            }

                            {
                                goto IL_01d7;
                            }
                        IL_3e4e:
                            num = 716;
                            List2.Visible = true;
                            goto IL_3e62;
                        IL_3e62:
                            num = 717;
                            List2.Items.Clear();
                            goto IL_3e7a;
                        IL_3e7a:
                            num = 718;
                            num7 = 1;
                            goto IL_3e84;
                        IL_3e84:
                            num = 719;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_3e9f;
                        IL_3e9f:
                            num = 722;
                            Label1[2].Text = "Fehlliste Orte";
                            goto IL_3ebd;
                        IL_3ebd:
                            num = 723;
                            Label1[1].Text = "      Nr    Ort Orsteil  Kreis Land Staat Loc  Länge  Breite PLZ Terr StKz";
                            goto IL_3edb;
                        IL_3edb:
                            num = 724;
                            Label1[0].Text = "  ";
                            goto IL_3ef9;
                        IL_3ef9:
                            num = 725;
                            List2.Items.Add("Fehlliste Orte");
                            goto IL_3f16;
                        IL_3f16:
                            num = 726;
                            List2.Items.Add("      Nr    Ort Ortsteil Kreis Land Staat Loc. Laenge Breite PLZ Terr. StKz");
                            goto IL_3f33;
                        IL_3f33:
                            num = 727;
                            DataModul.DB_PlaceTable.MoveFirst();
                            goto IL_3f45;
                        IL_3f45:
                            num = 728;
                            DataModul.DB_PlaceTable.Index = nameof(DataModul.PlaceIndex.OrtNr);
                            goto IL_3f5c;
                        IL_3f5c:
                            num = 729;
                            DataModul.DB_PlaceTable.Seek(">=", num7);
                            goto IL_3fb6;
                        IL_3fb6:
                            num = 730;
                            this.A = 1;
                            goto IL_3fc4;
                        IL_3fc4:
                            num = 731;
                            if (!DataModul.DB_PlaceTable.NoMatch)
                            {
                                goto IL_441a;
                            }
                            else
                            {
                                goto IL_4439;
                            }
                        IL_441a:
                            num = 733;
                            if (!DataModul.DB_PlaceTable.EOF)
                            {
                                goto IL_3fe6;
                            }
                            else
                            {
                                goto IL_4454;
                            }
                        IL_3fe6:
                            num = 734;
                            Modul1.LiText = new string(' ', 80);
                            goto IL_3ffb;
                        IL_3ffb:
                            num = 735;
                            StringType.MidStmtStr(ref Modul1.LiText, 1, 10, Strings.Right("          " + DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.OrtNr)].Value.AsString(), 10));
                            goto IL_4044;
                        IL_4044:
                            num = 736;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Ort)].Value, 0, TextCompare: false))
                            {
                                goto IL_4076;
                            }
                            else
                            {
                                goto IL_4091;
                            }
                        IL_4076:
                            num = 737;
                            StringType.MidStmtStr(ref Modul1.LiText, 12, 2, "F ");
                            goto IL_4091;
                        IL_4091:
                            num = 739;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Ortsteil)].Value, 0, TextCompare: false))
                            {
                                goto IL_40c3;
                            }
                            else
                            {
                                goto IL_40de;
                            }
                        IL_40c3:
                            num = 740;
                            StringType.MidStmtStr(ref Modul1.LiText, 20, 2, "F ");
                            goto IL_40de;
                        IL_40de:
                            num = 742;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Kreis)].Value, 0, TextCompare: false))
                            {
                                goto IL_4110;
                            }
                            else
                            {
                                goto IL_412b;
                            }
                        IL_4110:
                            num = 743;
                            StringType.MidStmtStr(ref Modul1.LiText, 28, 2, "F ");
                            goto IL_412b;
                        IL_412b:
                            num = 745;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Land)].Value, 0, TextCompare: false))
                            {
                                goto IL_415d;
                            }
                            else
                            {
                                goto IL_4178;
                            }
                        IL_415d:
                            num = 746;
                            StringType.MidStmtStr(ref Modul1.LiText, 34, 2, "F ");
                            goto IL_4178;
                        IL_4178:
                            num = 748;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Staat)].Value, 0, TextCompare: false))
                            {
                                goto IL_41aa;
                            }
                            else
                            {
                                goto IL_41c5;
                            }
                        IL_41aa:
                            num = 749;
                            StringType.MidStmtStr(ref Modul1.LiText, 39, 2, "F ");
                            goto IL_41c5;
                        IL_41c5:
                            num = 751;
                            if (Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Loc)].AsString()) == "")
                            {
                                goto IL_4203;
                            }
                            else
                            {
                                goto IL_421e;
                            }
                        IL_4203:
                            num = 752;
                            StringType.MidStmtStr(ref Modul1.LiText, 44, 2, "F ");
                            goto IL_421e;
                        IL_421e:
                            num = 754;
                            if (Strings.Len(Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.L)].AsString())) <= 1)
                            {
                                goto IL_4259;
                            }
                            else
                            {
                                goto IL_4274;
                            }
                        IL_4259:
                            num = 755;
                            StringType.MidStmtStr(ref Modul1.LiText, 52, 2, "F ");
                            goto IL_4274;
                        IL_4274:
                            num = 757;
                            if (Strings.Len(Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.B)].AsString())) <= 1)
                            {
                                goto IL_42af;
                            }
                            else
                            {
                                goto IL_42ca;
                            }
                        IL_42af:
                            num = 758;
                            StringType.MidStmtStr(ref Modul1.LiText, 58, 2, "F ");
                            goto IL_42ca;
                        IL_42ca:
                            num = 760;
                            if (Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.PLZ)].AsString()) == "")
                            {
                                goto IL_4308;
                            }
                            else
                            {
                                goto IL_4323;
                            }
                        IL_4308:
                            num = 761;
                            StringType.MidStmtStr(ref Modul1.LiText, 63, 2, "F ");
                            goto IL_4323;
                        IL_4323:
                            num = 763;
                            if (Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Terr)].AsString()) == "")
                            {
                                goto IL_4361;
                            }
                            else
                            {
                                goto IL_437c;
                            }
                        IL_4361:
                            num = 764;
                            StringType.MidStmtStr(ref Modul1.LiText, 68, 2, "F ");
                            goto IL_437c;
                        IL_437c:
                            num = 766;
                            if (Strings.Trim(DataModul.DB_PlaceTable.Fields[nameof(DataModul.PlaceFields.Staatk)].AsString()) == "")
                            {
                                goto IL_43ba;
                            }
                            else
                            {
                                goto IL_43d5;
                            }
                        IL_43ba:
                            num = 767;
                            StringType.MidStmtStr(ref Modul1.LiText, 71, 2, "F ");
                            goto IL_43d5;
                        IL_43d5:
                            num = 769;
                            List2.Items.Add(Modul1.LiText);
                            goto IL_43f2;
                        IL_43f2:
                            num = 770;
                            Modul1.LiText = new string(' ', 80);
                            goto IL_4407;
                        IL_4407:
                            num = 771;
                            DataModul.DB_PlaceTable.MoveNext();
                            goto IL_441a;
                        IL_4439:
                            num = 774;
                            goto IL_4441;
                        IL_4441:
                            num = 775;
                            DataModul.DB_PlaceTable.MoveNext();
                            goto IL_4454;
                        IL_4454:
                            num = 777;
                            if (DataModul.DB_PlaceTable.EOF)
                            {
                                goto IL_446b;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_446b:
                            num = 778;
                            List2.Items.Add("Ende der Liste");
                            goto IL_448a;
                        IL_3406:
                            num = 618;
                            List1.Items.Clear();
                            goto IL_341e;
                        IL_341e:
                            num = 619;
                            Label1[2].Text = Modul1.IText[84];
                            goto IL_343f;
                        IL_343f:
                            num = 620;
                            Label1[0].Text = "Name              Elternteil  Kind  Pate  Zeuge  Adoptivkind  verbunden";
                            goto IL_345d;
                        IL_345d:
                            num = 621;
                            num7 = 1;
                            goto IL_3467;
                        IL_3467:
                            num = 622;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_3482;
                        IL_3482:
                            num = 625;
                            DataModul.DB_PersonTable.Index = nameof(DataModul.PersonIndex.PerNr);
                            goto IL_3499;
                        IL_3499:
                            num = 626;
                            DataModul.DB_PersonTable.MoveLast();
                            goto IL_34ac;
                        IL_34ac:
                            num = 627;
                            num8 = DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value.AsInt();
                            goto IL_34d3;
                        IL_34d3:
                            num = 628;
                            if (num7 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_34eb;
                        IL_34eb:
                            num = 631;
                            if (Operators.ConditionalCompareObjectGreater(num7, DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value, TextCompare: false))
                            {
                                goto IL_351e;
                            }
                            else
                            {
                                goto IL_355f;
                            }
                        IL_351e:
                            num = 632;
                            Interaction.MsgBox(Modul1.IText[173] + " " + num7.AsString() + Modul1.IText[172]);
                            goto end_IL_0001_2;
                        IL_355f:
                            num = 635;
                            Label1[1].Text = "unvollständig verknüpfte Personen ";
                            goto IL_357d;
                        IL_357d:
                            num = 636;
                            Modul1.Schalt = 3;
                            goto IL_358a;
                        IL_358a:
                            num = 637;
                            ProgressBar1.Minimum = 0;
                            goto IL_359e;
                        IL_359e:
                            num = 638;
                            ProgressBar1.Maximum = 0;
                            goto IL_35b2;
                        IL_35b2:
                            num = 639;
                            ProgressBar1.Step = 1;
                            goto IL_35c6;
                        IL_35c6:
                            num = 640;
                            ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                            goto IL_35e5;
                        IL_35e5:
                            num = 641;
                            i3 = num7;
                            num9 = DataModul.DB_PersonTable.RecordCount - 1;
                            I1 = i3;
                            goto IL_3dfe;
                        IL_3dfe:
                            i4 = I1;
                            num5 = num9;
                            if (i4 <= num5)
                            {
                                goto IL_3607;
                            }
                            else
                            {
                                goto IL_3e0f;
                            }
                        IL_3e0f:
                            num = 713;
                            Label4.Text = List1.Items.Count - 1.AsString() + " Einträge";
                            goto IL_448a;
                        IL_3607:
                            num = 642;
                            ProgressBar1.PerformStep();
                            goto IL_361a;
                        IL_361a:
                            num = 643;
                            Application.DoEvents();
                            goto IL_3627;
                        IL_3627:
                            num = 644;
                            Modul1.PersInArb = I1;
                            goto IL_3639;
                        IL_3639:
                            num = 645;
                            sDest = new string(' ', 80);
                            goto IL_364b;
                        IL_364b:
                            num = 646;
                            if (Modul1.PersInArb <= num8)
                            {
                                goto IL_3666;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_3666:
                            num = 649;
                            DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                            goto IL_36c8;
                        IL_36c8:
                            num = 650;
                            if (!DataModul.DB_PersonTable.NoMatch)
                            {
                                goto IL_36e4;
                            }
                            else
                            {
                                goto IL_3de8;
                            }
                        IL_36e4:
                            num = 653;
                            LiKi = 1;
                            goto IL_36f2;
                        IL_36f2:
                            num = 654;
                            LiEl = 1;
                            goto IL_3700;
                        IL_3700:
                            num = 655;
                            //LiPa = 1;
                            goto IL_370e;
                        IL_370e:
                            num = 656;
                            Modul1.Kenn = 3f;
                            goto IL_371f;
                        IL_371f:
                            num = 657;
                            DataModul.DB_LinkTable.Index = nameof(DataModul.LinkIndex.ElSu);
                            goto IL_3736;
                        IL_3736:
                            num = 658;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), Modul1.Kenn);
                            goto IL_379d;
                        IL_379d:
                            num = 659;
                            if (DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_37b4;
                            }
                            else
                            {
                                goto IL_37c2;
                            }
                        IL_37b4:
                            num = 660;
                            LiKi = 0;
                            goto IL_37c2;
                        IL_37c2:
                            num = 662;
                            Modul1.Kenn = 1f;
                            goto IL_37d3;
                        IL_37d3:
                            num = 663;
                            Modul1.PerSatzLes();
                            goto IL_37e0;
                        IL_37e0:
                            num = 664;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.Sex)].Value, "F", TextCompare: false))
                            {
                                goto IL_3811;
                            }
                            else
                            {
                                goto IL_3822;
                            }
                        IL_3811:
                            num = 665;
                            Modul1.Kenn = 2f;
                            goto IL_3822;
                        IL_3822:
                            num = 667;
                            DataModul.DB_LinkTable.Index = nameof(DataModul.LinkIndex.ElSu);
                            goto IL_3839;
                        IL_3839:
                            num = 668;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), Modul1.Kenn);
                            goto IL_38a0;
                        IL_38a0:
                            num = 669;
                            if (DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_38b7;
                            }
                            else
                            {
                                goto IL_38c5;
                            }
                        IL_38b7:
                            num = 670;
                            LiEl = 0;
                            goto IL_38c5;
                        IL_38c5:
                            num = 672;
                            if ((LiKi == 0) & (LiEl == 0))
                            {
                                goto IL_38e8;
                            }
                            else
                            {
                                goto IL_3de8;
                            }
                        IL_38e8:
                            num = 673;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_38f5;
                        IL_38f5:
                            num = 674;
                            StringType.MidStmtStr(ref sDest, 1, 20, Strings.Left(Strings.Trim(Modul1.Kont[0]) + "," + Strings.Trim(Modul1.Kont[3]) + "                                  ", 20));
                            goto IL_3935;
                        IL_3935:
                            num = 675;
                            if (LiEl == 0)
                            {
                                goto IL_394b;
                            }
                            else
                            {
                                goto IL_3962;
                            }
                        IL_394b:
                            num = 676;
                            StringType.MidStmtStr(ref sDest, 24, 1, "N");
                            goto IL_3962;
                        IL_3962:
                            num = 678;
                            if (LiKi == 0)
                            {
                                goto IL_3978;
                            }
                            else
                            {
                                goto IL_398f;
                            }
                        IL_3978:
                            num = 679;
                            StringType.MidStmtStr(ref sDest, 30, 1, "N");
                            goto IL_398f;
                        IL_398f:
                            num = 681;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 4);
                            goto IL_39f2;
                        IL_39f2:
                            num = 682;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3a0c;
                            }
                            else
                            {
                                goto IL_3a23;
                            }
                        IL_3a0c:
                            num = 683;
                            StringType.MidStmtStr(ref sDest, 36, 1, "J");
                            goto IL_3a23;
                        IL_3a23:
                            num = 685;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 5);
                            goto IL_3a86;
                        IL_3a86:
                            num = 686;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3aa0;
                            }
                            else
                            {
                                goto IL_3ab7;
                            }
                        IL_3aa0:
                            num = 687;
                            StringType.MidStmtStr(ref sDest, 46, 1, "J");
                            goto IL_3ab7;
                        IL_3ab7:
                            num = 689;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 6);
                            goto IL_3b1a;
                        IL_3b1a:
                            num = 690;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3b34;
                            }
                            else
                            {
                                goto IL_3b4b;
                            }
                        IL_3b34:
                            num = 691;
                            StringType.MidStmtStr(ref sDest, 46, 1, "J");
                            goto IL_3b4b;
                        IL_3b4b:
                            num = 693;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 7);
                            goto IL_3bae;
                        IL_3bae:
                            num = 694;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3bc8;
                            }
                            else
                            {
                                goto IL_3bdf;
                            }
                        IL_3bc8:
                            num = 695;
                            StringType.MidStmtStr(ref sDest, 46, 1, "J");
                            goto IL_3bdf;
                        IL_3bdf:
                            num = 697;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 8);
                            goto IL_3c42;
                        IL_3c42:
                            num = 698;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3c5c;
                            }
                            else
                            {
                                goto IL_3c73;
                            }
                        IL_3c5c:
                            num = 699;
                            StringType.MidStmtStr(ref sDest, 55, 1, "J");
                            goto IL_3c73;
                        IL_3c73:
                            num = 701;
                            DataModul.DB_LinkTable.Seek("=", Modul1.PersInArb.AsString(), 9);
                            goto IL_3cd7;
                        IL_3cd7:
                            num = 702;
                            if (!DataModul.DB_LinkTable.NoMatch)
                            {
                                goto IL_3cf1;
                            }
                            else
                            {
                                goto IL_3d08;
                            }
                        IL_3cf1:
                            num = 703;
                            StringType.MidStmtStr(ref sDest, 70, 1, "J");
                            goto IL_3d08;
                        IL_3d08:
                            num = 705;
                            DataModul.DB_WitnessTable.Index = nameof(DataModul.WitnessIndex.ElSu);
                            goto IL_3d1f;
                        IL_3d1f:
                            num = 706;
                            DataModul.DB_WitnessTable.Seek("=", Modul1.PersInArb, "10");
                            goto IL_3d7c;
                        IL_3d7c:
                            num = 707;
                            if (!DataModul.DB_WitnessTable.NoMatch)
                            {
                                goto IL_3d96;
                            }
                            else
                            {
                                goto IL_3dad;
                            }
                        IL_3d96:
                            num = 708;
                            StringType.MidStmtStr(ref sDest, 46, 1, "J");
                            goto IL_3dad;
                        IL_3dad:
                            num = 710;
                            List1.Items.Add(new ListItem(sDest + "          " + Modul1.PersInArb.AsString()));
                            goto IL_3de8;
                        IL_3de8:
                            num = 712;
                            I1++;
                            goto IL_3dfe;
                        IL_2496:
                            num = 439;
                            List1.Items.Clear();
                            goto IL_24ae;
                        IL_24ae:
                            num = 440;
                            Label1[2].Text = "Personen ohne Datum";
                            goto IL_24cc;
                        IL_24cc:
                            num = 441;
                            Label1[1].Text = Modul1.IText[166];
                            goto IL_24f0;
                        IL_24f0:
                            num = 442;
                            Label1[0].Text = Modul1.IText[167];
                            goto IL_2514;
                        IL_2514:
                            num = 443;
                            I1 = 0;
                            goto IL_2522;
                        IL_2522:
                            num = 444;
                            Label1[(short)I1].Refresh();
                            goto IL_2541;
                        IL_2541:
                            num = 445;
                            I1++;
                            i5 = I1;
                            num5 = 2;
                            if (i5 <= num5)
                            {
                                goto IL_2522;
                            }
                            else
                            {
                                goto IL_2564;
                            }
                        IL_2564:
                            num = 446;
                            num7 = 1;
                            goto IL_256e;
                        IL_256e:
                            num = 447;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_2589;
                        IL_2589:
                            num = 450;
                            DataModul.DB_PersonTable.Index = nameof(DataModul.PersonIndex.PerNr);
                            goto IL_25a0;
                        IL_25a0:
                            num = 451;
                            DataModul.DB_PersonTable.MoveLast();
                            goto IL_25b3;
                        IL_25b3:
                            num = 452;
                            num8 = DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value.AsInt();
                            goto IL_25da;
                        IL_25da:
                            num = 453;
                            if (num7 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_25f2;
                        IL_25f2:
                            num = 456;
                            if (Operators.ConditionalCompareObjectGreater(num7, DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value, TextCompare: false))
                            {
                                goto IL_2625;
                            }
                            else
                            {
                                goto IL_2666;
                            }
                        IL_2625:
                            num = 457;
                            Interaction.MsgBox(Modul1.IText[173] + " " + num7.AsString() + Modul1.IText[172]);
                            goto end_IL_0001_2;
                        IL_2666:
                            num = 460;
                            ProgressBar1.Minimum = 0;
                            goto IL_267a;
                        IL_267a:
                            num = 461;
                            ProgressBar1.Maximum = 0;
                            goto IL_268e;
                        IL_268e:
                            num = 462;
                            ProgressBar1.Step = 1;
                            goto IL_26a2;
                        IL_26a2:
                            num = 463;
                            ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                            goto IL_26c1;
                        IL_26c1:
                            num = 464;
                            i6 = num7;
                            num10 = DataModul.DB_PersonTable.RecordCount - 1;
                            I1 = i6;
                            goto IL_33b6;
                        IL_33b6:
                            i7 = I1;
                            num5 = num10;
                            if (i7 <= num5)
                            {
                                goto IL_26e3;
                            }
                            else
                            {
                                goto IL_33c7;
                            }
                        IL_33c7:
                            num = 615;
                            Label4.Text = List1.Items.Count - 1.AsString() + " Einträge";
                            goto IL_448a;
                        IL_26e3:
                            num = 465;
                            ProgressBar1.PerformStep();
                            goto IL_26f6;
                        IL_26f6:
                            num = 466;
                            Application.DoEvents();
                            goto IL_2703;
                        IL_2703:
                            num = 467;
                            Modul1.PersInArb = I1;
                            goto IL_2715;
                        IL_2715:
                            num = 468;
                            if (Modul1.PersInArb <= num8)
                            {
                                goto IL_2730;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_2730:
                            num = 471;
                            DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                            goto IL_2792;
                        IL_2792:
                            num = 472;
                            if (!DataModul.DB_PersonTable.NoMatch)
                            {
                                goto IL_27ae;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_27ae:
                            num = 475;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_27bb;
                        IL_27bb:
                            num = 476;
                            Modul1.LiText = Strings.Left(Strings.Trim(Modul1.Kont[0]) + "," + Strings.Trim(Modul1.Kont[3]) + "                                  ", 20);
                            goto IL_27f5;
                        IL_27f5:
                            num = 477;
                            Modul1.Schalt = 20;
                            goto IL_2803;
                        IL_2803:
                            num = 478;
                            T = 1;
                            goto IL_2811;
                        IL_2811:
                            num = 479;
                            Modul1.Kont[T + 10] = "";
                            goto IL_282d;
                        IL_282d:
                            num = 480;
                            Modul1.Kont[T + 20] = "";
                            goto IL_2849;
                        IL_2849:
                            num = 481;
                            T = (short)unchecked(T + 1);
                            t = T;
                            num11 = 10;
                            if (t <= num11)
                            {
                                goto IL_2811;
                            }
                            else
                            {
                                goto IL_286e;
                            }
                        IL_286e:
                            num = 482;
                            Modul1.Datles();
                            goto IL_287b;
                        IL_287b:
                            num = 483;
                            sDest2 = "                                                                            ";
                            goto IL_2889;
                        IL_2889:
                            num = 484;
                            if (Strings.Trim(Modul1.Kont[11]) == "")
                            {
                                goto IL_28b9;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_28b9:
                            num = 487;
                            if (Strings.Trim(Modul1.Kont[12]) == "")
                            {
                                goto IL_28e9;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_28e9:
                            num = 490;
                            if (Strings.Trim(Modul1.Kont[13]) == "")
                            {
                                goto IL_2919;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2919:
                            num = 493;
                            if (Strings.Trim(Modul1.Kont[14]) == "")
                            {
                                goto IL_2949;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2949:
                            num = 496;
                            num12 = 300;
                            goto IL_2956;
                        IL_2956:
                            num = 497;
                            DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.BeSu);
                            goto IL_296d;
                        IL_296d:
                            num = 498;
                            DataModul.DB_EventTable.Seek("=", num12, Modul1.PersInArb.AsString());
                            goto IL_29d0;
                        IL_29d0:
                            num = 499;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_29ec;
                            }
                            else
                            {
                                goto IL_2d63;
                            }
                        IL_29ec:
                            num = 502;
                            switch (num12)
                            {
                                case 300:
                                    break;
                                case 301:
                                    goto IL_2b31;
                                case 302:
                                    goto IL_2c4c;
                                default:
                                    goto IL_2d63;
                            }

                            {
                                goto IL_2a16;
                            }
                        IL_2c4c:
                            num = 534;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_2c7e;
                            }
                            else
                            {
                                goto IL_2c9b;
                            }
                        IL_2c7e:
                            num = 535;
                            StringType.MidStmtStr(ref sDest2, 50, 1, Modul1.IText[175]);
                            goto IL_2c9b;
                        IL_2c9b:
                            num = 537;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_2cd2;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2cd2:
                            num = 540;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_2d09;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2d09:
                            num = 543;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_2d44;
                            }
                            else
                            {
                                goto IL_2d63;
                            }
                        IL_2d44:
                            num = 544;
                            StringType.MidStmtStr(ref sDest2, 53, 1, Modul1.IText[175]);
                            goto IL_2d63;
                        IL_2b31:
                            num = 520;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_2b63;
                            }
                            else
                            {
                                goto IL_2b80;
                            }
                        IL_2b63:
                            num = 521;
                            StringType.MidStmtStr(ref sDest2, 42, 1, Modul1.IText[175]);
                            goto IL_2b80;
                        IL_2b80:
                            num = 523;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_2bb7;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2bb7:
                            num = 526;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_2bee;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2bee:
                            num = 529;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_2c29;
                            }
                            else
                            {
                                goto IL_2d63;
                            }
                        IL_2c29:
                            num = 530;
                            StringType.MidStmtStr(ref sDest2, 46, 1, Modul1.IText[175]);
                            goto IL_2d63;
                        IL_2a16:
                            num = 506;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_2a48;
                            }
                            else
                            {
                                goto IL_2a65;
                            }
                        IL_2a48:
                            num = 507;
                            StringType.MidStmtStr(ref sDest2, 34, 1, Modul1.IText[175]);
                            goto IL_2a65;
                        IL_2a65:
                            num = 509;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_2a9c;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2a9c:
                            num = 512;
                            if (!Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_2ad3;
                            }
                            else
                            {
                                goto IL_3395;
                            }
                        IL_2ad3:
                            num = 515;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_2b0e;
                            }
                            else
                            {
                                goto IL_2d63;
                            }
                        IL_2b0e:
                            num = 516;
                            StringType.MidStmtStr(ref sDest2, 38, 1, Modul1.IText[175]);
                            goto IL_2d63;
                        IL_2d63:
                            num = 547;
                            lErl = 92;
                            goto IL_2d6e;
                        IL_2d6e:
                            num = 548;
                            num12 = (short)unchecked(num12 + 1);
                            num13 = num12;
                            num11 = 302;
                            if (num13 <= num11)
                            {
                                goto IL_2956;
                            }
                            else
                            {
                                goto IL_32fc;
                            }
                        IL_32fc:
                            num = 606;
                            lErl = 45;
                            goto IL_3307;
                        IL_3307:
                            num = 607;
                            Modul1.LiText += sDest2;
                            goto IL_3320;
                        IL_3320:
                            num = 609;
                            if (Strings.Trim(Modul1.LiText) != "")
                            {
                                goto IL_3348;
                            }
                            else
                            {
                                goto IL_3383;
                            }
                        IL_3348:
                            num = 610;
                            List1.Items.Add(new ListItem(Modul1.LiText + "          " + Modul1.PersInArb.AsString()));
                            goto IL_3383;
                        IL_3383:
                            num = 612;
                            Modul1.LiText = "";
                            goto IL_3395;
                        IL_3395:
                            num = 613;
                            lErl = 95;
                            goto IL_33a0;
                        IL_33a0:
                            num = 614;
                            I1++;
                            goto IL_33b6;
                        IL_1c00:
                            num = 336;
                            //Sz = 0;
                            goto IL_1c0e;
                        IL_1c0e:
                            num = 337;
                            List1.Items.Clear();
                            goto IL_1c26;
                        IL_1c26:
                            num = 338;
                            Label1[2].Text = "Familien ohne Datum";
                            goto IL_1c44;
                        IL_1c44:
                            num = 339;
                            Label1[1].Text = "Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. Eheä. Aus  Fikt.";
                            goto IL_1c62;
                        IL_1c62:
                            num = 340;
                            Label1[0].Text = "                                    DO   DO    DO    DO   DO    DO        DO";
                            goto IL_1c80;
                        IL_1c80:
                            num = 341;
                            if (RadioButton2.Checked)
                            {
                                goto IL_1c98;
                            }
                            else
                            {
                                goto IL_1cb5;
                            }
                        IL_1c98:
                            num = 342;
                            List1.Items.Add("Familien ohne Datum");
                            goto IL_1cb5;
                        IL_1cb5:
                            num = 344;
                            if (RadioButton2.Checked)
                            {
                                goto IL_1ccd;
                            }
                            else
                            {
                                goto IL_1cea;
                            }
                        IL_1ccd:
                            num = 345;
                            List1.Items.Add("Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.");
                            goto IL_1cea;
                        IL_1cea:
                            num = 347;
                            if (RadioButton2.Checked)
                            {
                                goto IL_1d02;
                            }
                            else
                            {
                                goto IL_1d1f;
                            }
                        IL_1d02:
                            num = 348;
                            List1.Items.Add("                                    DO   DO    DO    DO   DO    DO        DO");
                            goto IL_1d1f;
                        IL_1d1f:
                            num = 350;
                            Modul1.I = 0;
                            goto IL_1d2c;
                        IL_1d2c:
                            num = 351;
                            Label1[(short)Modul1.I].Refresh();
                            goto IL_1d4a;
                        IL_1d4a:
                            num = 352;
                            Modul1.I++;
                            i8 = Modul1.I;
                            num5 = 2;
                            if (i8 <= num5)
                            {
                                goto IL_1d2c;
                            }
                            else
                            {
                                goto IL_1d6a;
                            }
                        IL_1d6a:
                            num = 353;
                            num7 = 1;
                            goto IL_1d74;
                        IL_1d74:
                            num = 354;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_1d8f;
                        IL_1d8f:
                            num = 357;
                            DataModul.DB_FamilyTable.MoveLast();
                            goto IL_1da2;
                        IL_1da2:
                            num = 358;
                            num8 = DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.FamNr)].Value.AsInt();
                            goto IL_1dc9;
                        IL_1dc9:
                            num = 359;
                            if (Operators.ConditionalCompareObjectGreater(num7, DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.FamNr)].Value, TextCompare: false))
                            {
                                goto IL_1dfc;
                            }
                            else
                            {
                                goto IL_1e3d;
                            }
                        IL_1dfc:
                            num = 360;
                            Interaction.MsgBox(Modul1.IText[174] + " " + num7.AsString() + Modul1.IText[172]);
                            goto end_IL_0001_2;
                        IL_1e3d:
                            num = 363;
                            ProgressBar1.Minimum = 0;
                            goto IL_1e51;
                        IL_1e51:
                            num = 364;
                            ProgressBar1.Maximum = 0;
                            goto IL_1e65;
                        IL_1e65:
                            num = 365;
                            ProgressBar1.Step = 1;
                            goto IL_1e79;
                        IL_1e79:
                            num = 366;
                            ProgressBar1.Maximum = (int)Math.Round(Conversion.Val(Modul1.Aus[13]));
                            goto IL_1e9f;
                        IL_1e9f:
                            num = 367;
                            i9 = num7;
                            num14 = num7 + DataModul.DB_FamilyTable.RecordCount;
                            I1 = i9;
                            goto IL_2440;
                        IL_2440:
                            i10 = I1;
                            num5 = num14;
                            if (i10 <= num5)
                            {
                                goto IL_1ec2;
                            }
                            else
                            {
                                goto IL_2451;
                            }
                        IL_2451:
                            num = 435;
                            Label4.Text = List1.Items.Count - 2.AsString() + " Einträge";
                            goto end_IL_0001_2;
                        IL_1ec2:
                            num = 368;
                            ProgressBar1.PerformStep();
                            goto IL_1ed5;
                        IL_1ed5:
                            num = 369;
                            Application.DoEvents();
                            goto IL_1ee2;
                        IL_1ee2:
                            num = 370;
                            sDest2 = "                                                                                          ";
                            goto IL_1ef0;
                        IL_1ef0:
                            num = 371;
                            Modul1.FamInArb = I1;
                            goto IL_1f02;
                        IL_1f02:
                            num = 372;
                            if (Modul1.FamInArb <= num8)
                            {
                                goto IL_1f1d;
                            }
                            else
                            {
                                goto IL_241f;
                            }
                        IL_1f1d:
                            num = 375;
                            DataModul.DB_FamilyTable.Index = nameof(DataModul.FamilyIndex.Fam);
                            goto IL_1f34;
                        IL_1f34:
                            num = 376;
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
                            goto IL_1f96;
                        IL_1f96:
                            num = 377;
                            if (!DataModul.DB_FamilyTable.NoMatch)
                            {
                                goto IL_1fb2;
                            }
                            else
                            {
                                goto IL_2317;
                            }
                        IL_1fb2:
                            num = 380;
                            MainProject.Forms.Familie.Famles(Modul1.FamInArb);
                            goto IL_1fbf;
                        IL_1fbf:
                            num = 381;
                            if (Modul1.Family.Mann > 0)
                            {
                                goto IL_1fd7;
                            }
                            else
                            {
                                goto IL_206d;
                            }
                        IL_1fd7:
                            num = 382;
                            Modul1.PersInArb = Modul1.Family.Mann;
                            goto IL_1fe8;
                        IL_1fe8:
                            num = 383;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_1ff5;
                        IL_1ff5:
                            num = 384;
                            if (Modul1.Kont[0] != "")
                            {
                                goto IL_201a;
                            }
                            else
                            {
                                goto IL_2035;
                            }
                        IL_201a:
                            num = 385;
                            StringType.MidStmtStr(ref sDest2, 1, 15, Modul1.Kont[0]);
                            goto IL_2055;
                        IL_2035:
                            num = 387;
                            goto IL_203d;
                        IL_203d:
                            num = 388;
                            StringType.MidStmtStr(ref sDest2, 1, 15, ">NN<");
                            goto IL_2055;
                        IL_2055:
                            num = 390;
                            StringType.MidStmtStr(ref sDest2, 16, 2, "/ ");
                            goto IL_206d;
                        IL_206d:
                            num = 392;
                            if (Modul1.Family.Frau > 0)
                            {
                                goto IL_2085;
                            }
                            else
                            {
                                goto IL_2106;
                            }
                        IL_2085:
                            num = 393;
                            Modul1.PersInArb = Modul1.Family.Frau;
                            goto IL_2096;
                        IL_2096:
                            num = 394;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_20a3;
                        IL_20a3:
                            num = 395;
                            if (Modul1.Kont[0] != "")
                            {
                                goto IL_20c8;
                            }
                            else
                            {
                                goto IL_20e4;
                            }
                        IL_20c8:
                            num = 396;
                            StringType.MidStmtStr(ref sDest2, 17, 15, Modul1.Kont[0]);
                            goto IL_2106;
                        IL_20e4:
                            num = 398;
                            goto IL_20ec;
                        IL_20ec:
                            num = 399;
                            StringType.MidStmtStr(ref sDest2, 17, 15, ">NN<");
                            goto IL_2106;
                        IL_2106:
                            num = 402;
                            if (Modul1.Family.Kind[1] != 0)
                            {
                                goto IL_2120;
                            }
                            else
                            {
                                goto IL_213d;
                            }
                        IL_2120:
                            num = 403;
                            StringType.MidStmtStr(ref sDest2, 32, 1, Modul1.IText[175]);
                            goto IL_213d;
                        IL_213d:
                            num = 405;
                            DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.ArtNr);
                            goto IL_2154;
                        IL_2154:
                            num = 406;
                            DataModul.DB_FamilyTable.Index = nameof(DataModul.FamilyIndex.Fam);
                            goto IL_216b;
                        IL_216b:
                            num = 407;
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                            goto IL_21c8;
                        IL_21c8:
                            num = 408;
                            if (!DataModul.DB_FamilyTable.NoMatch)
                            {
                                goto IL_21e4;
                            }
                            else
                            {
                                goto IL_2317;
                            }
                        IL_21e4:
                            num = 411;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.Aeb)].Value, -1, TextCompare: false))
                            {
                                goto IL_2216;
                            }
                            else
                            {
                                goto IL_2233;
                            }
                        IL_2216:
                            num = 412;
                            StringType.MidStmtStr(ref sDest2, 70, 1, Modul1.IText[175]);
                            goto IL_2233;
                        IL_2233:
                            num = 414;
                            num15 = 500;
                            goto IL_2240;
                        IL_2240:
                            num = 415;
                            Modul1.Ubg = num15;
                            goto IL_224d;
                        IL_224d:
                            num = 416;
                            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.FamInArb.AsString(), "0");
                            goto IL_22b9;
                        IL_22b9:
                            num = 417;
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_22d8;
                            }
                            else
                            {
                                goto IL_241f;
                            }
                        IL_22d8:
                            num = 420;
                            if (!((num15 == 507) & DataModul.DB_EventTable.NoMatch))
                            {
                                goto IL_22fa;
                            }
                            else
                            {
                                goto IL_2317;
                            }
                        IL_22fa:
                            num = 423;
                            num15 = (short)unchecked(num15 + 1);
                            num16 = num15;
                            num11 = 507;
                            if (num16 <= num11)
                            {
                                goto IL_2240;
                            }
                            else
                            {
                                goto IL_2317;
                            }
                        IL_2317:
                            num = 424;
                            lErl = 34;
                            goto IL_2322;
                        IL_2322:
                            num = 425;
                            DataModul.DB_EventTable.Seek("=", 601.AsString(), Modul1.FamInArb.AsString(), "0");
                            goto IL_238e;
                        IL_238e:
                            num = 426;
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_23aa;
                            }
                            else
                            {
                                goto IL_241f;
                            }
                        IL_23aa:
                            num = 429;
                            if (sDest2.Trim() != "")
                            {
                                goto IL_23cf;
                            }
                            else
                            {
                                goto IL_241f;
                            }
                        IL_23cf:
                            num = 430;
                            List1.Items.Add(new ListItem(sDest2 + "          " + Modul1.FamInArb.AsString()));
                            goto IL_2407;
                        IL_2407:
                            num = 431;
                            //Sz++;
                            goto IL_241f;
                        IL_241f:
                            num = 433;
                            lErl = 35;
                            goto IL_242a;
                        IL_242a:
                            num = 434;
                            I1++;
                            goto IL_2440;
                        IL_1aae:
                            num = 322;
                            FileSystem.FileClose(99);
                            goto IL_1acb;
                        IL_1acb:
                            num = 323;
                            FileSystem.FileOpen(99, Modul1.TempPath + "\\Text4.Txt", OpenMode.Output);
                            goto IL_1aed;
                        IL_1aed:
                            num = 324;
                            FileSystem.PrintLine(99, Label1[2].Text);
                            goto IL_1b1c;
                        IL_1b1c:
                            num = 325;
                            FileSystem.PrintLine(99, Label1[1].Text);
                            goto IL_1b4b;
                        IL_1b4b:
                            num = 326;
                            FileSystem.PrintLine(99, Label1[0].Text);
                            goto IL_1b7a;
                        IL_1b7a:
                            num = 327;
                            if (List2.Visible)
                            {
                                goto IL_1b92;
                            }
                            else
                            {
                                goto IL_1bb9;
                            }
                        IL_1b92:
                            num = 328;
                            L = List2;
                            A = 2;
                            Modul1.Listbox3Clip(ref L, ref A);
                            List2 = L;
                            goto IL_1be7;
                        IL_1bb9:
                            num = 330;
                            goto IL_1bc1;
                        IL_1bc1:
                            num = 331;
                            L = List1;
                            A = 2;
                            Modul1.Listbox3Clip(ref L, ref A);
                            List1 = L;
                            goto IL_1be7;
                        IL_1be7:
                            num = 333;
                            FileSystem.FileClose();
                            goto IL_448a;
                        IL_1a3c:
                            num = 314;
                            List1.Items.Clear();
                            goto IL_1a54;
                        IL_1a54:
                            num = 315;
                            List2.Items.Clear();
                            goto IL_1a6c;
                        IL_1a6c:
                            num = 316;
                            ProgressBar1.Minimum = 0;
                            goto IL_1a80;
                        IL_1a80:
                            num = 317;
                            ProgressBar1.Maximum = 0;
                            goto IL_1a94;
                        IL_1a94:
                            num = 318;
                            Close();
                            goto end_IL_0001_2;
                        IL_0cef:
                            num = 159;
                            DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.ArtNr);
                            goto IL_0d06;
                        IL_0d06:
                            num = 160;
                            List1.Items.Clear();
                            goto IL_0d1e;
                        IL_0d1e:
                            num = 161;
                            Label1[2].Text = "Fehlliste Familien";
                            goto IL_0d3c;
                        IL_0d3c:
                            num = 162;
                            Label1[1].Text = "Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.";
                            goto IL_0d5a;
                        IL_0d5a:
                            num = 163;
                            Label1[0].Text = "                                    DO   DO    DO    DO   DO    DO        DO";
                            goto IL_0d78;
                        IL_0d78:
                            num = 164;
                            if (RadioButton2.Checked)
                            {
                                goto IL_0d90;
                            }
                            else
                            {
                                goto IL_0dad;
                            }
                        IL_0d90:
                            num = 165;
                            List1.Items.Add("Fehlliste Familien");
                            goto IL_0dad;
                        IL_0dad:
                            num = 167;
                            if (RadioButton2.Checked)
                            {
                                goto IL_0dc5;
                            }
                            else
                            {
                                goto IL_0de2;
                            }
                        IL_0dc5:
                            num = 168;
                            List1.Items.Add("Mann            Frau       Kinder  Pro  Verl. Hei   k.H. Schd. eheä. auß  Fikt.");
                            goto IL_0de2;
                        IL_0de2:
                            num = 170;
                            if (RadioButton2.Checked)
                            {
                                goto IL_0dfa;
                            }
                            else
                            {
                                goto IL_0e17;
                            }
                        IL_0dfa:
                            num = 171;
                            List1.Items.Add("                                    DO   DO    DO    DO   DO    DO        DO");
                            goto IL_0e17;
                        IL_0e17:
                            num = 173;
                            Modul1.I = 0;
                            goto IL_0e24;
                        IL_0e24:
                            num = 174;
                            Label1[(short)Modul1.I].Refresh();
                            goto IL_0e42;
                        IL_0e42:
                            num = 175;
                            Modul1.I++;
                            i11 = Modul1.I;
                            num5 = 2;
                            if (i11 <= num5)
                            {
                                goto IL_0e24;
                            }
                            else
                            {
                                goto IL_0e62;
                            }
                        IL_0e62:
                            num = 176;
                            num7 = 1;
                            goto IL_0e6c;
                        IL_0e6c:
                            num = 177;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_0e87;
                        IL_0e87:
                            num = 180;
                            DataModul.DB_FamilyTable.MoveLast();
                            goto IL_0e9a;
                        IL_0e9a:
                            num = 181;
                            num8 = DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.FamNr)].Value.AsInt();
                            goto IL_0ec1;
                        IL_0ec1:
                            num = 182;
                            if (Operators.ConditionalCompareObjectGreater(num7, DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.FamNr)].Value, TextCompare: false))
                            {
                                goto IL_0ef4;
                            }
                            else
                            {
                                goto IL_0f36;
                            }
                        IL_0ef4:
                            num = 183;
                            Interaction.MsgBox("Die höchste Familiennummer ist " + DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.FamNr)].Value.AsString());
                            goto end_IL_0001_2;
                        IL_0f36:
                            num = 186;
                            ProgressBar1.Minimum = 0;
                            goto IL_0f4a;
                        IL_0f4a:
                            num = 187;
                            ProgressBar1.Maximum = DataModul.DB_FamilyTable.RecordCount - 1;
                            goto IL_0f69;
                        IL_0f69:
                            num = 188;
                            ProgressBar1.Step = 1;
                            goto IL_0f7d;
                        IL_0f7d:
                            num = 189;
                            i12 = num7;
                            num17 = num8;
                            I1 = i12;
                            goto IL_199d;
                        IL_199d:
                            i13 = I1;
                            num5 = num17;
                            if (i13 <= num5)
                            {
                                goto IL_0f95;
                            }
                            else
                            {
                                goto IL_19ae;
                            }
                        IL_19ae:
                            num = 308;
                            Label4.Text = List1.Items.Count - 3.AsString() + " Familien";
                            goto IL_19e7;
                        IL_19e7:
                            num = 309;
                            if (RadioButton1.Checked)
                            {
                                goto IL_19ff;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_19ff:
                            num = 310;
                            Label4.Text = List1.Items.Count.AsString() + " Familien";
                            goto IL_448a;
                        IL_0f95:
                            num = 190;
                            ProgressBar1.PerformStep();
                            goto IL_0fa8;
                        IL_0fa8:
                            num = 191;
                            Application.DoEvents();
                            goto IL_0fb5;
                        IL_0fb5:
                            num = 192;
                            sDest2 = "                                                                                          ";
                            goto IL_0fc3;
                        IL_0fc3:
                            num = 193;
                            Modul1.FamInArb = I1;
                            goto IL_0fd5;
                        IL_0fd5:
                            num = 194;
                            if (Modul1.FamInArb <= num8)
                            {
                                goto IL_0ff0;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_0ff0:
                            num = 197;
                            DataModul.DB_FamilyTable.Index = nameof(DataModul.FamilyIndex.Fam);
                            goto IL_1007;
                        IL_1007:
                            num = 198;
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb.AsString());
                            goto IL_1069;
                        IL_1069:
                            num = 199;
                            if (!DataModul.DB_FamilyTable.NoMatch)
                            {
                                goto IL_1085;
                            }
                            else
                            {
                                goto IL_197c;
                            }
                        IL_1085:
                            num = 202;
                            MainProject.Forms.Familie.Famles(Modul1.FamInArb);
                            goto IL_1092;
                        IL_1092:
                            num = 203;
                            if (Modul1.Family.Mann > 0)
                            {
                                goto IL_10aa;
                            }
                            else
                            {
                                goto IL_1140;
                            }
                        IL_10aa:
                            num = 204;
                            Modul1.PersInArb = Modul1.Family.Mann;
                            goto IL_10bb;
                        IL_10bb:
                            num = 205;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_10c8;
                        IL_10c8:
                            num = 206;
                            if (Modul1.Kont[0] != "")
                            {
                                goto IL_10ed;
                            }
                            else
                            {
                                goto IL_1108;
                            }
                        IL_10ed:
                            num = 207;
                            StringType.MidStmtStr(ref sDest2, 1, 15, Modul1.Kont[0]);
                            goto IL_1128;
                        IL_1108:
                            num = 209;
                            goto IL_1110;
                        IL_1110:
                            num = 210;
                            StringType.MidStmtStr(ref sDest2, 1, 15, ">NN<");
                            goto IL_1128;
                        IL_1128:
                            num = 212;
                            StringType.MidStmtStr(ref sDest2, 16, 2, "/ ");
                            goto IL_1140;
                        IL_1140:
                            num = 214;
                            if (Modul1.Family.Frau > 0)
                            {
                                goto IL_1158;
                            }
                            else
                            {
                                goto IL_11d9;
                            }
                        IL_1158:
                            num = 215;
                            Modul1.PersInArb = Modul1.Family.Frau;
                            goto IL_1169;
                        IL_1169:
                            num = 216;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_1176;
                        IL_1176:
                            num = 217;
                            if (Modul1.Kont[0] != "")
                            {
                                goto IL_119b;
                            }
                            else
                            {
                                goto IL_11b7;
                            }
                        IL_119b:
                            num = 218;
                            StringType.MidStmtStr(ref sDest2, 17, 15, Modul1.Kont[0]);
                            goto IL_11d9;
                        IL_11b7:
                            num = 220;
                            goto IL_11bf;
                        IL_11bf:
                            num = 221;
                            StringType.MidStmtStr(ref sDest2, 17, 15, ">NN<");
                            goto IL_11d9;
                        IL_11d9:
                            num = 224;
                            if (Modul1.Family.Kind[1] != 0)
                            {
                                goto IL_11f3;
                            }
                            else
                            {
                                goto IL_1210;
                            }
                        IL_11f3:
                            num = 225;
                            StringType.MidStmtStr(ref sDest2, 32, 1, Modul1.IText[175]);
                            goto IL_1210;
                        IL_1210:
                            num = 227;
                            DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.ArtNr);
                            goto IL_1227;
                        IL_1227:
                            num = 228;
                            DataModul.DB_FamilyTable.Index = nameof(DataModul.FamilyIndex.Fam);
                            goto IL_123e;
                        IL_123e:
                            num = 229;
                            DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                            goto IL_129b;
                        IL_129b:
                            num = 230;
                            if (!DataModul.DB_FamilyTable.NoMatch)
                            {
                                goto IL_12b7;
                            }
                            else
                            {
                                goto IL_197c;
                            }
                        IL_12b7:
                            num = 233;
                            if (Operators.ConditionalCompareObjectEqual(DataModul.DB_FamilyTable.Fields[nameof(DataModul.FamilyFields.Aeb)].Value, -1, TextCompare: false))
                            {
                                goto IL_12e9;
                            }
                            else
                            {
                                goto IL_1306;
                            }
                        IL_12e9:
                            num = 234;
                            StringType.MidStmtStr(ref sDest2, 70, 1, Modul1.IText[175]);
                            goto IL_1306;
                        IL_1306:
                            num = 236;
                            num15 = 500;
                            goto IL_1313;
                        IL_1313:
                            num = 237;
                            Modul1.Ubg = num15;
                            goto IL_1320;
                        IL_1320:
                            num = 238;
                            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.FamInArb.AsString(), "0");
                            goto IL_138c;
                        IL_138c:
                            num = 239;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_13a8;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_13a8:
                            num = 242;
                            switch (Modul1.Ubg)
                            {
                                case 500:
                                    break;
                                case 501:
                                    goto IL_148f;
                                case 502:
                                    goto IL_153c;
                                case 503:
                                    goto IL_15e9;
                                case 504:
                                    goto IL_1696;
                                case 505:
                                    goto IL_1743;
                                default:
                                    goto IL_17ec;
                            }

                            {
                                goto IL_13e2;
                            }
                        IL_1743:
                            num = 286;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_1775;
                            }
                            else
                            {
                                goto IL_1792;
                            }
                        IL_1775:
                            num = 287;
                            StringType.MidStmtStr(ref sDest2, 65, 1, Modul1.IText[175]);
                            goto IL_1792;
                        IL_1792:
                            num = 289;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_17cd;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_17cd:
                            num = 290;
                            StringType.MidStmtStr(ref sDest2, 66, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_1696:
                            num = 278;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_16c8;
                            }
                            else
                            {
                                goto IL_16e5;
                            }
                        IL_16c8:
                            num = 279;
                            StringType.MidStmtStr(ref sDest2, 59, 1, Modul1.IText[175]);
                            goto IL_16e5;
                        IL_16e5:
                            num = 281;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_1720;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_1720:
                            num = 282;
                            StringType.MidStmtStr(ref sDest2, 60, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_15e9:
                            num = 270;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_161b;
                            }
                            else
                            {
                                goto IL_1638;
                            }
                        IL_161b:
                            num = 271;
                            StringType.MidStmtStr(ref sDest2, 54, 1, Modul1.IText[175]);
                            goto IL_1638;
                        IL_1638:
                            num = 273;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_1673;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_1673:
                            num = 274;
                            StringType.MidStmtStr(ref sDest2, 55, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_153c:
                            num = 262;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_156e;
                            }
                            else
                            {
                                goto IL_158b;
                            }
                        IL_156e:
                            num = 263;
                            StringType.MidStmtStr(ref sDest2, 48, 1, Modul1.IText[175]);
                            goto IL_158b;
                        IL_158b:
                            num = 265;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_15c6;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_15c6:
                            num = 266;
                            StringType.MidStmtStr(ref sDest2, 49, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_148f:
                            num = 254;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_14c1;
                            }
                            else
                            {
                                goto IL_14de;
                            }
                        IL_14c1:
                            num = 255;
                            StringType.MidStmtStr(ref sDest2, 42, 1, Modul1.IText[175]);
                            goto IL_14de;
                        IL_14de:
                            num = 257;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_1519;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_1519:
                            num = 258;
                            StringType.MidStmtStr(ref sDest2, 43, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_13e2:
                            num = 246;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_1414;
                            }
                            else
                            {
                                goto IL_1431;
                            }
                        IL_1414:
                            num = 247;
                            StringType.MidStmtStr(ref sDest2, 37, 1, Modul1.IText[175]);
                            goto IL_1431;
                        IL_1431:
                            num = 249;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_146c;
                            }
                            else
                            {
                                goto IL_17ec;
                            }
                        IL_146c:
                            num = 250;
                            StringType.MidStmtStr(ref sDest2, 38, 1, Modul1.IText[175]);
                            goto IL_17ec;
                        IL_17ec:
                            num = 293;
                            lErl = 54;
                            goto IL_17f7;
                        IL_17f7:
                            num = 294;
                            num15 = (short)unchecked(num15 + 1);
                            num18 = num15;
                            num11 = 507;
                            if (num18 <= num11)
                            {
                                goto IL_1313;
                            }
                            else
                            {
                                goto IL_1814;
                            }
                        IL_1814:
                            num = 295;
                            lErl = 3;
                            goto IL_181e;
                        IL_181e:
                            num = 296;
                            DataModul.DB_EventTable.Seek("=", 601.AsString(), Modul1.FamInArb.AsString(), "0");
                            goto IL_188a;
                        IL_188a:
                            num = 297;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_18a7;
                            }
                            else
                            {
                                goto IL_1943;
                            }
                        IL_18a7:
                            num = 298;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_18d9;
                            }
                            else
                            {
                                goto IL_18f0;
                            }
                        IL_18d9:
                            num = 299;
                            StringType.MidStmtStr(ref sDest2, 75, 1, "J");
                            goto IL_18f0;
                        IL_18f0:
                            num = 301;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_192b;
                            }
                            else
                            {
                                goto IL_1943;
                            }
                        IL_192b:
                            num = 302;
                            StringType.MidStmtStr(ref sDest2, 76, 1, "J");
                            goto IL_1943;
                        IL_1943:
                            num = 305;
                            List1.Items.Add(new ListItem(sDest2 + "          " + Modul1.FamInArb.AsString()));
                            goto IL_197c;
                        IL_197c:
                            num = 306;
                            lErl = 55;
                            goto IL_1987;
                        IL_1987:
                            num = 307;
                            I1++;
                            goto IL_199d;
                        IL_01d7:
                            num = 35;
                            List1.Items.Clear();
                            goto IL_01ec;
                        IL_01ec:
                            num = 36;
                            Label1[2].Text = "Fehlliste Personen";
                            goto IL_0207;
                        IL_0207:
                            num = 37;
                            Label1[1].Text = Modul1.IText[166];
                            goto IL_0228;
                        IL_0228:
                            num = 38;
                            Label1[0].Text = Modul1.IText[167];
                            goto IL_0249;
                        IL_0249:
                            num = 39;
                            Label1[1].Text = Label1[1].Text + " Quelle";
                            goto IL_027a;
                        IL_027a:
                            num = 40;
                            Label1[0].Text = Label1[0].Text + "    T V";
                            goto IL_02ab;
                        IL_02ab:
                            num = 41;
                            I1 = 0;
                            goto IL_02b6;
                        IL_02b6:
                            num = 42;
                            Label1[(short)I1].Refresh();
                            goto IL_02d2;
                        IL_02d2:
                            num = 43;
                            I1++;
                            i14 = I1;
                            num5 = 2;
                            if (i14 <= num5)
                            {
                                goto IL_02b6;
                            }
                            else
                            {
                                goto IL_02f2;
                            }
                        IL_02f2:
                            num = 44;
                            num7 = 1;
                            goto IL_02f9;
                        IL_02f9:
                            num = 45;
                            if (num7 <= 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_0311;
                        IL_0311:
                            num = 48;
                            DataModul.DB_PersonTable.Index = nameof(DataModul.PersonIndex.PerNr);
                            goto IL_0325;
                        IL_0325:
                            num = 49;
                            DataModul.DB_PersonTable.MoveLast();
                            goto IL_0335;
                        IL_0335:
                            num = 50;
                            num8 = DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value.AsInt();
                            goto IL_0359;
                        IL_0359:
                            num = 51;
                            if (num7 == 0)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_036e;
                        IL_036e:
                            num = 54;
                            if (Operators.ConditionalCompareObjectGreater(num7, DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value, TextCompare: false))
                            {
                                goto IL_039e;
                            }
                            else
                            {
                                goto IL_03dd;
                            }
                        IL_039e:
                            num = 55;
                            Interaction.MsgBox("Die höchste Personennummer ist " + DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.PersNr)].Value.AsString());
                            goto end_IL_0001_2;
                        IL_03dd:
                            num = 58;
                            ProgressBar1.Minimum = 0;
                            goto IL_03ee;
                        IL_03ee:
                            num = 59;
                            ProgressBar1.Maximum = 0;
                            goto IL_03ff;
                        IL_03ff:
                            num = 60;
                            ProgressBar1.Step = 1;
                            goto IL_0410;
                        IL_0410:
                            num = 61;
                            ProgressBar1.Maximum = DataModul.DB_PersonTable.RecordCount - 1;
                            goto IL_042c;
                        IL_042c:
                            num = 62;
                            List1.Items.Clear();
                            goto IL_0441;
                        IL_0441:
                            num = 63;
                            i15 = num7;
                            num19 = num8;
                            I1 = i15;
                            goto IL_0ca1;
                        IL_0ca1:
                            i16 = I1;
                            num5 = num19;
                            if (i16 <= num5)
                            {
                                goto IL_0456;
                            }
                            else
                            {
                                goto IL_0cb2;
                            }
                        IL_0cb2:
                            num = 156;
                            Label4.Text = List1.Items.Count.AsString() + " Einträge";
                            goto IL_448a;
                        IL_0456:
                            num = 64;
                            ProgressBar1.PerformStep();
                            goto IL_0466;
                        IL_0466:
                            num = 65;
                            Application.DoEvents();
                            goto IL_0470;
                        IL_0470:
                            num = 66;
                            Modul1.PersInArb = I1;
                            goto IL_047f;
                        IL_047f:
                            num = 67;
                            if (Modul1.PersInArb <= num8)
                            {
                                goto IL_0497;
                            }
                            else
                            {
                                goto IL_448a;
                            }
                        IL_448a:
                            num = 781;
                            lErl = 4;
                            goto IL_4494;
                        IL_4494:
                            num = 782;
                            if (!RadioButton2.Checked)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_44ac;
                        IL_44ac:
                            num = 783;
                            List1.Items.Add("Ende der Liste");
                            goto IL_44c9;
                        IL_44c9:
                            num = 784;
                            Modul1.I = 1;
                            goto IL_44d6;
                        IL_44d6:
                            num = 785;
                            List1.Items.Add("");
                            goto IL_44f3;
                        IL_44f3:
                            num = 786;
                            Modul1.I++;
                            i17 = Modul1.I;
                            num5 = 17;
                            if (i17 > num5)
                            {
                                goto end_IL_0001_2;
                            }
                            goto IL_44d6;
                        IL_0497:
                            num = 70;
                            DataModul.DB_PersonTable.Seek("=", Modul1.PersInArb.AsString());
                            goto IL_04f6;
                        IL_04f6:
                            num = 71;
                            if (!DataModul.DB_PersonTable.NoMatch)
                            {
                                goto IL_050f;
                            }
                            else
                            {
                                goto IL_0c81;
                            }
                        IL_050f:
                            num = 74;
                            Modul1.Personlesen(Modul1.PersInArb);
                            goto IL_0519;
                        IL_0519:
                            num = 75;
                            Modul1.LiText = Strings.Left(Strings.Trim(Modul1.Kont[0]) + "," + Strings.Trim(Modul1.Kont[3]) + "                                  ", 20);
                            goto IL_0550;
                        IL_0550:
                            num = 76;
                            Modul1.Schalt = 20;
                            goto IL_055b;
                        IL_055b:
                            num = 77;
                            T = 1;
                            goto IL_0566;
                        IL_0566:
                            num = 78;
                            Modul1.Kont[T + 10] = " ";
                            goto IL_057f;
                        IL_057f:
                            num = 79;
                            Modul1.Kont[T + 20] = " ";
                            goto IL_0598;
                        IL_0598:
                            num = 80;
                            T = (short)unchecked(T + 1);
                            t2 = T;
                            num11 = 10;
                            if (t2 <= num11)
                            {
                                goto IL_0566;
                            }
                            else
                            {
                                goto IL_05ba;
                            }
                        IL_05ba:
                            num = 81;
                            Modul1.Datles();
                            goto IL_05c4;
                        IL_05c4:
                            num = 82;
                            sDest2 = "                                                                            ";
                            goto IL_05cf;
                        IL_05cf:
                            num = 83;
                            StringType.MidStmtStr(ref sDest2, 2, 2, Modul1.Kont[11] + Modul1.Kont[21]);
                            goto IL_05f2;
                        IL_05f2:
                            num = 84;
                            StringType.MidStmtStr(ref sDest2, 8, 2, Modul1.Kont[12] + Modul1.Kont[22]);
                            goto IL_0615;
                        IL_0615:
                            num = 85;
                            StringType.MidStmtStr(ref sDest2, 17, 2, Modul1.Kont[13] + Modul1.Kont[23]);
                            goto IL_0639;
                        IL_0639:
                            num = 86;
                            StringType.MidStmtStr(ref sDest2, 27, 2, Modul1.Kont[14] + Modul1.Kont[24]);
                            goto IL_065d;
                        IL_065d:
                            num = 87;
                            num12 = 300;
                            goto IL_0667;
                        IL_0667:
                            num = 88;
                            DataModul.DB_EventTable.Index = nameof(DataModul.EventIndex.BeSu);
                            goto IL_067b;
                        IL_067b:
                            num = 89;
                            DataModul.DB_EventTable.Seek("=", num12, Modul1.PersInArb.AsString());
                            goto IL_06db;
                        IL_06db:
                            num = 90;
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                goto IL_06f4;
                            }
                            else
                            {
                                goto IL_0ac2;
                            }
                        IL_06f4:
                            num = 93;
                            switch (num12)
                            {
                                case 300:
                                    break;
                                case 301:
                                    goto IL_084e;
                                case 302:
                                    goto IL_0981;
                                default:
                                    goto IL_0ac2;
                            }

                            {
                                goto IL_071b;
                            }
                        IL_0981:
                            num = 125;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_09b0;
                            }
                            else
                            {
                                goto IL_09ca;
                            }
                        IL_09b0:
                            num = 126;
                            StringType.MidStmtStr(ref sDest2, 50, 1, Modul1.IText[175]);
                            goto IL_09ca;
                        IL_09ca:
                            num = 128;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_09fc;
                            }
                            else
                            {
                                goto IL_0a19;
                            }
                        IL_09fc:
                            num = 129;
                            StringType.MidStmtStr(ref sDest2, 51, 1, Modul1.IText[175]);
                            goto IL_0a19;
                        IL_0a19:
                            num = 131;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_0a4b;
                            }
                            else
                            {
                                goto IL_0a68;
                            }
                        IL_0a4b:
                            num = 132;
                            StringType.MidStmtStr(ref sDest2, 52, 1, Modul1.IText[175]);
                            goto IL_0a68;
                        IL_0a68:
                            num = 134;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_0aa3;
                            }
                            else
                            {
                                goto IL_0ac2;
                            }
                        IL_0aa3:
                            num = 135;
                            StringType.MidStmtStr(ref sDest2, 53, 1, Modul1.IText[175]);
                            goto IL_0ac2;
                        IL_084e:
                            num = 111;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_087d;
                            }
                            else
                            {
                                goto IL_0897;
                            }
                        IL_087d:
                            num = 112;
                            StringType.MidStmtStr(ref sDest2, 42, 1, Modul1.IText[175]);
                            goto IL_0897;
                        IL_0897:
                            num = 114;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_08c6;
                            }
                            else
                            {
                                goto IL_08e0;
                            }
                        IL_08c6:
                            num = 115;
                            StringType.MidStmtStr(ref sDest2, 44, 1, Modul1.IText[175]);
                            goto IL_08e0;
                        IL_08e0:
                            num = 117;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_090f;
                            }
                            else
                            {
                                goto IL_0929;
                            }
                        IL_090f:
                            num = 118;
                            StringType.MidStmtStr(ref sDest2, 45, 1, Modul1.IText[175]);
                            goto IL_0929;
                        IL_0929:
                            num = 120;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_0961;
                            }
                            else
                            {
                                goto IL_0ac2;
                            }
                        IL_0961:
                            num = 121;
                            StringType.MidStmtStr(ref sDest2, 46, 1, Modul1.IText[175]);
                            goto IL_0ac2;
                        IL_071b:
                            num = 97;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.KBem)].Value, 0, TextCompare: false))
                            {
                                goto IL_074a;
                            }
                            else
                            {
                                goto IL_0764;
                            }
                        IL_074a:
                            num = 98;
                            StringType.MidStmtStr(ref sDest2, 34, 1, Modul1.IText[175]);
                            goto IL_0764;
                        IL_0764:
                            num = 100;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumV)].Value, 0, TextCompare: false))
                            {
                                goto IL_0793;
                            }
                            else
                            {
                                goto IL_07ad;
                            }
                        IL_0793:
                            num = 101;
                            StringType.MidStmtStr(ref sDest2, 36, 1, Modul1.IText[175]);
                            goto IL_07ad;
                        IL_07ad:
                            num = 103;
                            if (Operators.ConditionalCompareObjectGreater(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.DatumB)].Value, 0, TextCompare: false))
                            {
                                goto IL_07dc;
                            }
                            else
                            {
                                goto IL_07f6;
                            }
                        IL_07dc:
                            num = 104;
                            StringType.MidStmtStr(ref sDest2, 37, 1, Modul1.IText[175]);
                            goto IL_07f6;
                        IL_07f6:
                            num = 106;
                            if (Conversion.Val(DataModul.DB_EventTable.Fields[nameof(DataModul.EventFields.Ort)].Value) > 0.0)
                            {
                                goto IL_082e;
                            }
                            else
                            {
                                goto IL_0ac2;
                            }
                        IL_082e:
                            num = 107;
                            StringType.MidStmtStr(ref sDest2, 38, 1, Modul1.IText[175]);
                            goto IL_0ac2;
                        IL_0ac2:
                            num = 138;
                            lErl = 2;
                            goto IL_0acc;
                        IL_0acc:
                            num = 139;
                            num12 = (short)unchecked(num12 + 1);
                            num20 = num12;
                            num11 = 302;
                            if (num20 <= num11)
                            {
                                goto IL_0667;
                            }
                            else
                            {
                                goto IL_0ae8;
                            }
                        IL_0ae8:
                            num = 140;
                            Modul1.LiText += sDest2;
                            goto IL_0b00;
                        IL_0b00:
                            num = 141;
                            Modul1.PerSatzLes();
                            goto IL_0b0d;
                        IL_0b0d:
                            num = 142;
                            if (null != DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.Bem3)].Value)
                            {
                                goto IL_0b40;
                            }
                            else
                            {
                                goto IL_0b8b;
                            }
                        IL_0b40:
                            num = 143;
                            if (Operators.ConditionalCompareObjectNotEqual(DataModul.DB_PersonTable.Fields[nameof(DataModul.PersonFields.Bem3)].Value, "", TextCompare: false))
                            {
                                goto IL_0b71;
                            }
                            else
                            {
                                goto IL_0b8b;
                            }
                        IL_0b71:
                            num = 144;
                            StringType.MidStmtStr(ref Modul1.LiText, 77, 1, "J");
                            goto IL_0b8b;
                        IL_0b8b:
                            num = 147;
                            DataModul.TTable.Index = "Tab";
                            goto IL_0ba2;
                        end_IL_0001:
                            break;
                    }
                }
            }
            catch (Exception obj) when (num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 21059;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
