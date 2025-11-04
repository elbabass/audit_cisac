import { ConnectedComponent } from 'react-redux';

export interface IPrivateRouteProps {
  component: ConnectedComponent<any, any>;
  path: string;
  access?: boolean;
}
