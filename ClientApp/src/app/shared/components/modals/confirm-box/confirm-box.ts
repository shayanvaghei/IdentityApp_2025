import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-box',
  imports: [],
  templateUrl: './confirm-box.html',
  styleUrl: './confirm-box.scss'
})
export class ConfirmBox {
  @Input() message: string = '';
  result?: boolean;

  constructor(public activeModal: NgbActiveModal) { }

  yes() {
    this.activeModal.close(true);
  }

  no() {
    this.activeModal.close(false);
  }
}
