import cryptoRandomString = require('crypto-random-string');

export function randomString(length: number): string {
   return cryptoRandomString({ length: length });
}

export function getTitleForLargeResults(): string {
   const titles = ['adio', 'MAMMA MIA', 'yesterday', 'suspicious minds'];
   const i = Math.floor(Math.random() * Math.floor(titles.length));
   return titles[i];
}

export function formatIswc(iswc: string): string {
   return (
      iswc.charAt(0) + '-' + iswc.slice(1, 4) + '.' + iswc.slice(4, 7) + '.' + iswc.slice(7, 10) + '-' + iswc.charAt(10)
   );
}

export function unformatIswc(iswc: string): string {
   return iswc.replace(/[.-]/g, '');
}

export function getNewTitle() {
   return `UI TEST ${randomString(20)} ${randomString(20)}`;
}
