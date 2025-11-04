import { IInterestedParty, IIswcModel } from '../types/IswcTypes';

/**
 * Convert Array of objects into a dictionary
 * key: {object}
 * or
 * key: object.value
 */
export function normalizeArray<T>(array: Array<T>, indexKey: keyof T, value?: keyof T) {
  const normalizedObject: any = {};
  array.forEach((item) => {
    const key = item[indexKey];
    value ? (normalizedObject[key] = item[value]) : (normalizedObject[key] = item);
  });

  return normalizedObject as { [key: string]: T };
}

export function parseInterestedParties(
  surnames?: string,
  nameNumbers?: string,
  baseNumbers?: string,
): IInterestedParty[] {
  const interestedParties: IInterestedParty[] = [];
  const surnamesArr = surnames ? surnames.split(';') : [];
  const nameNumbersArr = nameNumbers ? nameNumbers.split(';') : [];
  const baseNumbersArr = baseNumbers ? baseNumbers.split(';') : [];
  const mainArr = [surnamesArr, nameNumbersArr, baseNumbersArr];
  const longestArrayIndex = getLongestArray(mainArr); 

  for (let x = 0; x < mainArr[longestArrayIndex].length; x++) {
    let ip: IInterestedParty = {
      lastName: surnamesArr[x] ? surnamesArr[x] : '',
      nameNumber: nameNumbersArr[x] ? parseInt(nameNumbersArr[x]) : undefined,
      baseNumber: baseNumbersArr[x] ? baseNumbersArr[x] : '',
      role: 'C',
    };

    interestedParties.push(ip);
  }

  return interestedParties;
}

function getLongestArray(array: string[][]) {
  return array
    .map(function (a) {
      return a.length;
    })
    .indexOf(
      Math.max.apply(
        Math,
        array.map(function (a) {
          return a.length;
        }),
      ),
    );
}

export function ensureResultIsArray(result: any): IIswcModel[] {
  let resultArr: IIswcModel[] = [];

  if (Array.isArray(result)) {
    if (result[0].searchId !== null && result[0].searchId !== undefined) {
      result.forEach((res) => resultArr.push(res.searchResults[0]));
    } else resultArr = result;
  } else resultArr = [result];

  return resultArr;
}

String.prototype.format = function (...args: string[]): string {
  var s = this;
  return s.replace(/{(\d+)}/g, function (match, number) {
    return typeof args[number] != 'undefined' ? args[number] : match;
  });
};
