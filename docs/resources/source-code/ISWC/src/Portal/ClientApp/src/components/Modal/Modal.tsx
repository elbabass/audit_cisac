import React, { memo } from 'react';
import styles from './Modal.module.scss';
import { IModalProps } from './ModalTypes';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import Loader from '../Loader/Loader';
import ActionButton from '../ActionButton/ActionButton';
import ErrorHandler from '../AlertMessage/ErrorHandler/ErrorHandler';
import BasicFormInput from '../FormInput/BasicFormInput';

const ModalComponent: React.FunctionComponent<IModalProps> = ({
  isModalOpen,
  toggleModal,
  headerText,
  bodyText,
  loading,
  error,
  leftButtonText,
  leftButtonAction,
  rightButtonText,
  rightButtonAction,
  type,
  onChangeInput,
  subHeaderText,
}) => {
  const renderInput = () => {
    return (
      <BasicFormInput
        name={'modalInput'}
        onChange={(event) => onChangeInput && onChangeInput(event.target.value)}
      />
    );
  };

  return (
    <Modal isOpen={isModalOpen} toggle={toggleModal} centered>
      <ModalHeader className={styles.modalHeader} toggle={toggleModal}>
        {headerText}
      </ModalHeader>
      <ModalBody className={styles.modalBody}>
        {subHeaderText && subHeaderText}
        {type === 'confirm' && bodyText}
        {type === 'input' && renderInput()}
        {loading && (
          <div className={styles.loader}>
            <Loader />
          </div>
        )}
        {error && <ErrorHandler error={error} />}
      </ModalBody>
      {!loading && (
        <ModalFooter className={styles.modalFooter}>
          {leftButtonText && (
            <div className={`${styles.modalActionButton} ${styles.actionButtonBig}`}>
              <ActionButton
                buttonText={leftButtonText}
                buttonAction={leftButtonAction && leftButtonAction}
                theme={'secondary'}
              />
            </div>
          )}
          <div className={`${styles.modalActionButton} ${styles.actionButtonBig}`}>
            <ActionButton buttonText={rightButtonText} buttonAction={rightButtonAction} />
          </div>
        </ModalFooter>
      )}
    </Modal>
  );
};

export default memo(ModalComponent);
