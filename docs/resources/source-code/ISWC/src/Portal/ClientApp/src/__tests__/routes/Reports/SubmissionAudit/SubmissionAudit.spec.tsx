import * as React from 'react';
import { mount, ReactWrapper } from 'enzyme';
import { ISubmissionAuditProps } from '../../../../routes/Reports/SubmissionAudit/SubmissionAuditTypes';
import SubmissionAudit from '../../../../routes/Reports/SubmissionAudit/SubmissionAudit';
import {
  SUBMISSION_AUDIT_BAR_CHART,
  SUBMISSION_AUDIT_EXTRACT_FTP,
  SUBMISSION_AUDIT_LIST_VIEW,
} from '../../../../consts';
import BarChart from '../../../../routes/Reports/SubmissionAudit/BarChart/BarChart';
import ExtractToFTP from '../../../../routes/Reports/ExtractToFTP/ExtractToFTP';
import {
  IAgencyStatisticsReportResult,
  ISubmissionAuditReportResult,
} from '../../../../redux/types/ReportTypes';

jest.mock('../../../../consts', () => {
  return {
    ...jest.requireActual('../../../../consts'),
    getAgencies: jest.fn(() => [{ name: 'abc', agencyId: '213' }]),
  };
});

describe('SubmissionAudit Page', () => {
  let props: ISubmissionAuditProps;
  let component: ReactWrapper;

  beforeEach(() => {
    props = {
      submissionAuditSearch: jest.fn(),
      loading: false,
      submissionAuditSearchResults: [],
      agencyStatisticsSearchResultsSubAudit: [],
      reportsAgencyStatisticsSearch: jest.fn(),
      extractToFtp: jest.fn(),
      extractToFtpSuccess: false,
      email: '',
      assignedRoles: {
        search: true,
        update: true,
        reportBasics: true,
        reportExtracts: true,
        reportAgencyInterest: true,
        reportIswcFullExtract: true,
        manageRoles: false,
      },
    };
    component = mount(<SubmissionAudit {...props} />);
  });

  it('renders without crashing', () => {
    expect(component.exists()).toBe(true);
  });

  it('should render the SubmissionAudit component displayView is SUBMISSION_AUDIT_LIST_VIEW', () => {
    component = mount(
      <SubmissionAudit
        {...props}
        submissionAuditSearchResults={[{} as ISubmissionAuditReportResult]}
      />,
    );
    component.setState({ displayView: SUBMISSION_AUDIT_LIST_VIEW });
    expect(component.find(SubmissionAudit).length).toBe(1);
  });

  it('should render the BarChart component displayView is SUBMISSION_AUDIT_BAR_CHART', () => {
    component = mount(
      <SubmissionAudit
        {...props}
        agencyStatisticsSearchResultsSubAudit={[{} as IAgencyStatisticsReportResult]}
      />,
    );
    component.setState({ displayView: SUBMISSION_AUDIT_BAR_CHART });
    expect(component.find(BarChart).length).toBe(1);
  });

  it('should render the ExtractToFTP component displayView is SUBMISSION_AUDIT_EXTRACT_FTP and extractToFtpSuccess is true', () => {
    component = mount(<SubmissionAudit {...props} extractToFtpSuccess={true} />);
    component.setState({ displayView: SUBMISSION_AUDIT_EXTRACT_FTP });
    expect(component.find(ExtractToFTP).length).toBe(1);
  });
});
