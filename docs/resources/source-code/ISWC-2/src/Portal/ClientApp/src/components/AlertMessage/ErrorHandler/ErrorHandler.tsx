import React, { memo } from 'react';
import AlertMessage from '../AlertMessage';
import { IErrorHandlerProps } from './ErrorHandlerTypes';

const ErrorHanlder: React.FunctionComponent<IErrorHandlerProps> = props => {
  const getErrorDetails = () => {
    const { error } = props;
    let message = 'An error has occurred';
    if (error.response && error.response.data) {
      if (error.response.data.message) message = error.response.data.message;
      else message = error.response.data;
    }
    return message;
  };

  return <AlertMessage message={getErrorDetails()} type={'error'} />;
};

export default memo(ErrorHanlder);
