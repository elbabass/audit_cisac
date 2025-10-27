import axios from 'axios';
export const validateReCaptchaResponse = (response: any) => {
  return new Promise<any>((resolve, reject) => {
    axios({
      method: 'POST',
      url: 'ReCaptcha/ValidateReCaptchaResponse',
      params: { responseToken: response },
    })
      .then((res: any) => {
        resolve(JSON.parse(res.data));
      })
      .catch((err: any) => {
        console.log(err);
        reject(err);
      });
  });
};
