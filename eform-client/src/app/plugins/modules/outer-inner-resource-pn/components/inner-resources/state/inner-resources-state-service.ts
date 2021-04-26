import { Injectable } from '@angular/core';
import { InnerResourcesStore } from './inner-resources-store';
import { Observable } from 'rxjs';
import { OperationDataResult } from 'src/app/common/models';
import { updateTableSort } from 'src/app/common/helpers';
import { getOffset } from 'src/app/common/helpers/pagination.helper';
import { map } from 'rxjs/operators';
import { InnerResourcesQuery } from './inner-resources-query';
import { OuterInnerResourcePnInnerResourceService } from '../../../services';
import { InnerResourcesPnModel } from '../../../models';

@Injectable({ providedIn: 'root' })
export class InnerResourcesStateService {
  constructor(
    private store: InnerResourcesStore,
    private service: OuterInnerResourcePnInnerResourceService,
    private query: InnerResourcesQuery
  ) {}

  private total: number;

  getAllMachines(): Observable<OperationDataResult<InnerResourcesPnModel>> {
    return this.service
      .getAllMachines({
        isSortDsc: this.query.pageSetting.isSortDsc,
        offset: this.query.pageSetting.offset,
        pageSize: this.query.pageSetting.pageSize,
        sort: this.query.pageSetting.sort,
        pageIndex: 0,
      })
      .pipe(
        map((response) => {
          if (response && response.success && response.model) {
            this.total = response.model.total;
          }
          return response;
        })
      );
  }

  updatePageSize(pageSize: number) {
    this.store.update({ pageSize: pageSize });
    this.checkOffset();
  }

  getOffset(): Observable<number> {
    return this.query.selectOffset$;
  }

  getPageSize(): Observable<number> {
    return this.query.selectPageSize$;
  }

  getSort(): Observable<string> {
    return this.query.selectSort$;
  }

  getIsSortDsc(): Observable<boolean> {
    return this.query.selectIsSortDsc$;
  }

  changePage(offset: number) {
    this.store.update({
      offset: offset,
    });
  }

  onDelete() {
    this.total -= 1;
    this.checkOffset();
  }

  onSortTable(sort: string) {
    const localPageSettings = updateTableSort(
      sort,
      this.query.pageSetting.sort,
      this.query.pageSetting.isSortDsc
    );
    this.store.update({
      isSortDsc: localPageSettings.isSortDsc,
      sort: localPageSettings.sort,
    });
  }

  checkOffset() {
    const newOffset = getOffset(
      this.query.pageSetting.pageSize,
      this.query.pageSetting.offset,
      this.total
    );
    if (newOffset !== this.query.pageSetting.offset) {
      this.store.update({
        offset: newOffset,
      });
    }
  }
}
