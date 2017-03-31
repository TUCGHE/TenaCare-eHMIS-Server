/*
 * @(#)EthiopicNumberConverter.java        1.0 10/04/2005
 * 
 * Copyright (c) 2005 Senamirmir Project. All Rights Reserved.
 *
 * This file is part of Software: Ealet 2.0
 *
 * Ealet 2.0 is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * Ealet 2.0 is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 */

/*
* 
* Copyright © 2006-2017 TenaCareeHMIS  software, by The Administrators of the Tulane Educational Fund, 
* dba Tulane University, Center for Global Health Equity is distributed under the GNU General Public License(GPL).
* All rights reserved.

* This file is part of TenaCareeHMIS
* TenaCareeHMIS is free software: 
* 
* you can redistribute it and/or modify it under the terms of the 
* GNU General Public License as published by the Free Software Foundation, 
* version 3 of the License, or any later version.
* TenaCareeHMIS is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
* FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.

* You should have received a copy of the GNU General Public License along with TenaCareeHMIS.  
* If not, see http://www.gnu.org/licenses/.    
* 
* 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
namespace General.Util.Ethiopia
{
/**
 * The Ethiopic numbering system is different from the Hindu-Arabic (modern) 
 * system. The system consists of 20 digits, but not 0, thus counting begins
 * from 1. The Ethiopic numerals in modern form are (1, 2, 3, 4, 5, 6, ,7, 8,
 * 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 10000).
 * <p>
 *
 * Briefly, these are the set of rules for writing Ethiopic numbers. Assume they 
 * are strictly based on integer (whole) numbers.
 * <ol>
 *    <li>Numbers from 1-10 are written as such</li>
 *    <li>Numbers from 11-99 are written using their tens place digits followed
 *        by the ones place digit. For instance, 11 = (10)(1).
 *    <li>Numbers from 100-9900 are written as a multiple of 100s plus the
 *        above rules
 *    </li>
 *    <li>Numbers from 10,000-above are written as a multiple of 10,000 plus
 *        the above rules
 *    </li>
 * </ol>
 *
 * @version 	1.0 October 12, 2005
 * @author 	abass alamnehe
 */
public class EthiopicNumber {
   
   /* the converted ethiopic digits are stored here, for about 128 */
   private long[] num = new long[128] ;
   
   /* position number fo the above array */
   private int index = 0 ;
   
   /* constant values used on the coversion process*/
   private const int TENTHOUSAND = 10000 ;
   private const int HUNDRED     = 100 ;
    private const int TEN = 10;

   /* for ethiopic digits mapping to modern and vs */
   private Hashtable ethiopicToModern = new Hashtable() ;
   private Hashtable modernToEthiopic = new Hashtable() ;
   
   /* Ethiopic digits in modern */
   private int[] ethiopicNumbersScale = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40,
                                50, 60, 70, 80, 90, 100, 10000} ;
   /* Ethiopic digits in Unicode */                                
   private char[] ethiopicNumbers = {'\u1369','\u136A','\u136B','\u136C','\u136D',
                             '\u136E','\u136F','\u1370','\u1371','\u1372',
                             '\u1373','\u1374','\u1375','\u1376','\u1377',
                             '\u1378','\u1379','\u137A','\u137B','\u137C'} ;
   
   /**
    * Instance with hashtable with Ethiopic digits and their equivalents
    */
   public EthiopicNumber() {
      /* maps Ethiopic digits to modern in hashtable */
      for (int i=0; i < ethiopicNumbers.Length; i++)
         ethiopicToModern.Add("" + ethiopicNumbers[i], "" + ethiopicNumbersScale[i]) ;
      
      /* maps Modern to Ethiopic in hashtable */
      for (int i=0; i < ethiopicNumbers.Length; i++)
          modernToEthiopic.Add("" + ethiopicNumbersScale[i], "" + ethiopicNumbers[i]);      
   } 
   
   /**
    * Maps the Ethiopic digit in modern and returns the result
    * 
    * @param c    The Ethiopic digit to be mapped to modern
    * @return     The equivalent number in modern to Ethiopic digit
    * @since      2.0
    */
   public int getModern(char c) {
      return ( Convert.ToInt32((string)ethiopicToModern["" + c]) ) ;      
   }

   /**
    * Maps modern number to Ethiopic digit and returns the result 
    *
    * @param num  The Ethiopic digit to be mapped to modern
    * @return     The equivalent Ethiopic digit to modern number
    * @since      2.0
    */
   public char getEthiopic(long num) {
      return (  ((string)modernToEthiopic["" + num])[0] ) ;      
   }

   /**
    * Returns Ethiopic number in Unicode form (2 bytes)
    * @param modern  A number in modern to be converted to Ethiopic
    * @return        Ethiopic number in Unicode (2bytes)
    * @since         2.0
    */
   public String ethiopicUnicode(long modern) {
      /* Ethiopic digit holder index is reset */
      index = 0 ;
      /* generates Ethiopic digits */
      toEthiopic(modern) ;
      /* digits holder */
      StringBuilder sb = new StringBuilder();
      
      /* fills the buffer with Ethiopic digits in Unicode form */
      for (int i=0; i < index; i++) {
         sb.Append( getEthiopic(num[i]) ) ;
      }

      return sb.ToString() ;
   }
   
   /**
    * Returns Ethiopic number in UTF-8
    * @param modern  A number in modern to be converted to Ethiopic
    * @return        Ethiopic number in UTF-8
    * @since         2.0
    */
   public String ethiopicUTF(long modern) {
      /* Ethiopic digit holder index is reset */
      index = 0 ;
      /* generates the digits */
      toEthiopic(modern) ;
      /* digits holder */
      StringBuilder sb = new StringBuilder() ;
      
      /* fills the buffer with the Ethiopic digits generated after mapping */
      for (int i=0; i < index; i++) {
         sb.Append( getEthiopic(num[i]) ) ;
      }

      /* transforms the digits into UTF-8 */
      byte[] temp = null ;
      String result = "" ;
      try {
         //temp = sb.ToString()getBytes("UTF-8") ;
          Encoding myUTF8 = Encoding.UTF8;
          temp = myUTF8.GetBytes(sb.ToString());
          result = myUTF8.GetString(temp);
      } catch (Exception uee) {
         Console.WriteLine(uee.Message) ;
      }
      return result ;
   }
      
   /**
    * Converets a Hindu-Arabic numerals to Ethiopic numerals and stores the
    * result into an array. The result is still in Hindu-Arabic numeral form;
    * that is, converstion to real Ethiopic digits is necessary.
    * <p>
    * 
    * @param x      Hindu-Arabic numeral to be converted
    * @since        2.0
    */
   public void toEthiopic(long x) {
      long r = 0, q = 0 ;
      
      if (x >= TENTHOUSAND) {       // x must be a multiple of 10,000
         r = x % TENTHOUSAND ;
         q = x / TENTHOUSAND ;
         if (q > 1) toEthiopic(q) ;
         num[index++] = TENTHOUSAND ;  
         if (r > 0) toEthiopic(r) ;        
      } else if (x >= HUNDRED) {    // x must be a multiple 100s
         r = x % HUNDRED ;
         q = x / HUNDRED ;
         if (q > 1) toEthiopic(q) ;
         num[index++] = HUNDRED ;
         if (r > 0) toEthiopic(r) ;           
      } else if (x >= TEN) {        // x must be a multiple 10s
         r = x % TEN ;
         q = x / TEN ;
         num[index++] = q * TEN ;
         if (r > 0) toEthiopic(r) ;         
         // System.out.println(index) ;
      } else {                      // x must be a multiple 1s
         num[index++] = x ;
         // System.out.println(index) ;         
      }
   }

   /**
    * Debugging function to track conversion results
    */
   void print() {
      for (int i=0; i < index; i++)
         Console.WriteLine("" + i + " : " + num[i] ) ;
   }
   
   /**
    * Converts Ethiopic number to modern or Hindu-Arabic number and returns the 
    * result. A given Ethiopic number is parsed from left to right while 
    * performing the arithmetic operations using stack. The following examples 
    * shows how such a given number is calculated to produce a modern equivalent.
    *
    * <pre>
    *    Unicode    :  0x1370 0x137C 0x1372 0x1371 0x137B 0xu137A 0x1371
    *    Decimal    :  2      10000   10     9      100    90      9
    *    Operations :  2  *   10000 +(10  +  9) *   100 +  90  +   9
    *    Result     :  21999 
    * </pre>
    *
    * @param ethiopic  Ethiopic number in String form
    * @return          The converted number result in modern
    * @since           2.0
    */   
   public long toModern(string ethiopic) {     
      /* stack is used to evaluate the expression */
      Stack<long> operands = new Stack<long>() ;
      /* token represents the result of numbers that are non-10,000 */
      long token = 0 ;

      /* Ethiopic number is parsed for left to right */
      for (int i=0; i < ethiopic.Length; i++) {
         /* converets a single digit into modern */
         int digit = getModern( ethiopic[i] ) ;         
         /* obtains access to the next digit */
         int lookAhead = (i == ethiopic.Length-1) 
                         ? -1 : getModern( ethiopic[i+1] ) ;         
         /* ensure that token is not 0 when it is multiplied by other digit */                           
         token = (token == 0 && digit == HUNDRED ? 1 : token) ;
         
         /* keeps track of digits, performs '+' or '*', push result to stack */
         if (digit < TENTHOUSAND) {            
            /* processes digits other than 10,000 */
            token = (digit == 100 ? token*digit : token+digit) ;
            /* if next is 10,000 or end, time to stack */
            if (lookAhead == TENTHOUSAND) {
               if (operands.Count != 0) {                  
                  operands.Push( token + operands.Pop()) ;                  
               } else
                  operands.Push(token) ;
            } else if (lookAhead == -1) {
               if (operands.Count != 0)               
                  operands.Push( token + operands.Pop()) ;
               else
                  operands.Push(token) ;                     
            }         
         } else if (digit == TENTHOUSAND) {   
            token = 0 ;            
            /* stack this digit */
            if ( operands.Count != 0) {               
               operands.Push( digit * operands.Pop()) ;
            } else
               operands.Push( digit ) ;
         }                        
      }
      return operands.Pop() ;                 
   }         
}
}