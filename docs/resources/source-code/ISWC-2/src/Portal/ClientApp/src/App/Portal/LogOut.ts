import { config } from '../../configuration/Configuration';
import axios from 'axios';
import Cookies from 'universal-cookie';

export function logOutUser() {
  axios.defaults.headers.common['Authorization'] = '';
  new Cookies().remove('authToken');
  window.location.href = config().loginRedirectUri;
}
