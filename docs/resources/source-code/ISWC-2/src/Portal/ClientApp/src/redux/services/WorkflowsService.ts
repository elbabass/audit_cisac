import axios from 'axios';
import { IWorkflow, IWorkflowSearchModel } from '../types/IswcTypes';
import { config } from '../../configuration/Configuration';

export const getWorkflows = (filters: IWorkflowSearchModel): Promise<IWorkflow[]> => {
  return new Promise<IWorkflow[]>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
      },
        method: 'get',
        url: config().iswcApiManagementUri + '/iswc/workflowTasks',
      params: {
        agency: filters.agency,
        showWorkflows: filters.showWorkflows,
        workflowType: filters.workflowType && parseInt(filters.workflowType),
        status: filters.statuses,
        startIndex: filters.startIndex,
        pageLength: filters.pageLength,
        fromDate: filters.fromDate,
        toDate: filters.toDate,
        iswc: filters.iswc,
        agencyWorkCodes: filters.agencyWorkCodes,
        originatingAgency: filters.originatingAgency,
      },
      paramsSerializer: (params) => {
        const paramsArr = new URLSearchParams();
        for (const key of Object.keys(params)) {
          const param = params[key];
          if (Array.isArray(param)) {
            for (const p of param) {
              paramsArr.append(key, p);
            }
          } else {
            paramsArr.append(key, param);
          }
        }
        return paramsArr.toString();
      },
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};

export const updateWorkflows = (agency?: string, requestBody?: any): Promise<IWorkflow[]> => {
  return new Promise<IWorkflow[]>((resolve, reject) => {
    axios({
      headers: {
        'Cache-Control': 'no-cache',
        Pragma: 'no-cache',
        'Content-Type': 'application/json',
      },
        method: 'patch',
        url: config().iswcApiManagementUri + '/iswc/workflowTasks',
      params: {
        agency,
      },
      data: requestBody,
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch((err) => {
        reject(err);
      });
  });
};
