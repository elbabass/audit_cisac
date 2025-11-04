import axios from 'axios';
import { IWebUser, IWebUserRole } from '../types/RoleTypes';

export const getLoggedInUserProfileRoles = (): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'get',
      url: 'Profile/GetUserRoles',
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};

export const getAllUsers = (): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'get',
      url: 'Profile/GetAllUsers',
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};

export const requestAccess = (requestedRole: IWebUserRole): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'post',
      url: 'Profile/RequestAccess',
      data: {
        IsApproved: requestedRole.isApproved,
        Role: requestedRole.role,
        RequestedDate: requestedRole.requestedDate,
        Status: requestedRole.status,
        Notification: requestedRole.notification,
      },
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};

export const getUserProfile = (user: IWebUser): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'post',
      url: 'Profile/GetUserProfile',
      data: {
        Email: user.email,
        AgencyId: user.agencyId,
        WebUserRoles: user.webUserRoles,
      },
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};

export const updateUserRole = (user: IWebUser): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'put',
      url: 'Profile/UpdateUserRole',
      data: {
        Email: user.email,
        AgencyId: user.agencyId,
        WebUserRoles: user.webUserRoles,
      },
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};

export const getAccessRequests = (): Promise<any> => {
  return new Promise<any>((resolve, reject) => {
    axios({
      headers: {
        'Content-Type': 'application/json',
      },
      method: 'get',
      url: 'Profile/GetAccessRequests',
    })
      .then((result) => {
        resolve(result);
      })
      .catch((error) => {
        if (error.response?.status === 503) {
          reject(error.response.status);
        } else {
          reject(error);
        }
      });
  });
};
