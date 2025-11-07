import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AdminService } from '../../../../admin/admin.service';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-ia-pagination-th',
  imports: [NgClass],
  templateUrl: './ia-pagination-th.html',
  styleUrl: './ia-pagination-th.scss'
})
export class IaPaginationTh implements OnInit {
  @Input() label?: string;
  @Input() sortByValue?: string;
  @Input() isSortable = false;
  @Output() sortBy = new EventEmitter<string>();
  arrowUp: boolean | undefined;

  constructor(private adminService: AdminService) {
  }

  ngOnInit(): void {
    this.adminService.resetFilter$.subscribe(_ => {
      this.arrowUp = undefined;
    });

    this.adminService.resetSortBy$.subscribe(result => {
      if (this.sortByValue !== result) {
        this.arrowUp = undefined;
      }
    });
  }

  toggleArrow() {
    if (this.isSortable && this.sortByValue) {
      this.adminService.resetSortBy(this.sortByValue);

      if (this.arrowUp === undefined) {
        this.arrowUp = true;
        this.sortBy.emit(this.sortByValue.toLocaleLowerCase().replace(/\s+/, "") + '_a');
      } else if (this.arrowUp === true) {
        this.arrowUp = false;
        this.sortBy.emit(this.sortByValue.toLocaleLowerCase().replace(/\s+/, "") + '_d');
      } else {
        this.arrowUp = undefined;
        this.sortBy.emit('');
      }
    }
  }
}
